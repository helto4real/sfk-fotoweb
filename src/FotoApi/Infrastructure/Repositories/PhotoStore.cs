using System.Text;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Image = SixLabors.ImageSharp.Image;
using System.IO.Compression;

namespace FotoApi.Infrastructure.Repositories;

/// <summary>
///     Manage the storage of photos on file system
/// </summary>
public class PhotoStore : IPhotoStore, IPhoto, IDisposable
{
    private readonly IWebHostEnvironment _env;
    private readonly CurrentUser? _currentUser;
    private readonly ILogger<PhotoStore> _logger;
    private Image? _image;
    private readonly string _photoUserFolderAndPath;
    private readonly string _imagePath;

    private const string ImageFolder = ".images";
    private const string ImageRootFolder = ".images/user_images";
    private const string StPackageRootFolder = ".images/st-packages";

    public PhotoStore(IWebHostEnvironment env, CurrentUser currentUser, ILogger<PhotoStore> logger)
    {
        _env = env;
        _currentUser = currentUser;
        _logger = logger;
        // We dont want to fail if no user exists here, it will be handled when saving the photo
        _photoUserFolderAndPath = GetPhotoUserFolderAndPath(_currentUser?.User?.Id ?? "nouser" );
        _imagePath =  GetPhotoFullPath(_env.ContentRootPath, _photoUserFolderAndPath);
    }

    public async Task<IPhoto?> SavePhotoAsync(Stream inputStream, (int, int) imageSize, bool resize)
    {
        var (width, height) = imageSize;
        
        if (_currentUser?.User is null)
            return null;

        if (!EnsureDirectoryExists(_imagePath))
            return null;

        _image =  await Image.LoadAsync(inputStream, CancellationToken.None);

        if (resize)
        {
            _logger.LogInformation("Image too large, resizing to {Width}x{Height}", width, height);
            ResizeImage(width, height);
        }
        else
        {
            if (_image.Width != width || _image.Height != height)
            {
                _logger.LogInformation("Image uploaded is the wrong size expected {Width}x{Height} but got {ActualWidth}x{ActualHeight}", width, height, _image.Width, _image.Height);  
                throw new WrongImageSizeException(width, height);
            }
        }
        await using (FileStream fs = new(_imagePath, FileMode.Create))
        {
            await _image.SaveAsync(fs, new JpegEncoder() { Quality = 80 });
        }  

        return this;
    }

    public void DeletePhoto(string  relativePath)
    {
        if (_env is null) return;
        if (_currentUser?.User?.Id is null) return;
        
        var fullPath = Path.Combine(_env.ContentRootPath, ImageRootFolder, relativePath);
        var fullThumbnailPath = ConvertToThumbnailPath(fullPath);
        
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        
        if (File.Exists(fullThumbnailPath))
            File.Delete(fullThumbnailPath);
    }

    private static string GetPhotoFullPath(string rootPath, string relativePath)
    {
        return Path.Combine(rootPath, ImageRootFolder, relativePath);
    }

    private static string GetPhotoUserFolderAndPath(string userId)
    {
        var trustedFileNameForFileStorage = Path.GetRandomFileName();

        return Path.Combine(userId,
            trustedFileNameForFileStorage) + ".jpg";
    }

    private static bool EnsureDirectoryExists(string imagePath)
    {
        var directory = Path.GetDirectoryName(imagePath);
        if (string.IsNullOrEmpty(directory))
            return false;
        Directory.CreateDirectory(directory);
        
        return true;
    }
    
    private void ResizeImage(int width, int height)
    {
        if (_image == null)
            throw new NullReferenceException("Image is null");
        
        if (_image.Width > width || _image.Height > height)
        {
            _image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max,
            }));
        }
    }
    
    private void ResizeThumbnailTo165X165Size()
    {
        if (_image == null)
            throw new NullReferenceException("Image is null");
        
        _image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(165, 165),
            Mode = ResizeMode.Max,
            Sampler = LanczosResampler.Lanczos2
        }));
    }

    private string ThumbnailPath => ConvertToThumbnailPath(_imagePath);

    private string ConvertToThumbnailPath(string imagePath)
    {
        return imagePath.Replace(".jpg", "_thumb.jpg");
    }
    
    public string PhotoUserFolderAndPath => _photoUserFolderAndPath;
    public async Task PackageStBild(string imageLocalRelativeFilePath, Guid packageId, StBild stBild)
    {
        var packageFolder = Path.Combine(StPackageRootFolder, packageId.ToString());
        if (!EnsureDirectoryExists(packageFolder))
        {
            _logger.LogError("Could not create directory {StPackageRootFolder}", StPackageRootFolder);
            return;
        }

        var destinationFilenameWithoutExtension = MakeFilenameSafe(stBild.Title);
        var destinationImageFileName = MakeFilenameUnique(destinationFilenameWithoutExtension + ".jpg");
        var destinationInfoFileName = MakeFilenameUnique(destinationFilenameWithoutExtension + ".txt");
        var destinationImageFilePath = Path.Combine(packageFolder, destinationImageFileName);
        var destinationInfoFilePath = Path.Combine(packageFolder, destinationInfoFileName);
        
        var imageSourcePath = Path.Combine(ImageRootFolder, imageLocalRelativeFilePath);
        
        if (!EnsureDirectoryExists(destinationImageFilePath))
        {
            _logger.LogError("Could not create directory {DestinationImageFilePath}", destinationImageFilePath);
            return;
        }
        
        await CopyFileAsync(imageSourcePath, destinationImageFilePath);
        await WriteStTextInfo(destinationInfoFilePath, stBild);
    }
    
    public string ZipPackage(Guid packageId, int packageNumber)
    {
        var packageFolder = Path.Combine(StPackageRootFolder, packageId.ToString());
        var zipFilePath = Path.Combine(StPackageRootFolder, $"SFK ST-bilder paket {packageNumber}" + ".zip");
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);
        
        ZipFile.CreateFromDirectory(packageFolder, zipFilePath);
        Directory.Delete(packageFolder, true);
        return zipFilePath;
    }

    private async Task WriteStTextInfo(string destinationPath, StBild stBild)
    {
        await using var destinationStream = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write,
            FileShare.None, bufferSize: 4096, useAsync: true);
        await using var writer = new StreamWriter(destinationStream);
        
        await writer.WriteLineAsync("Fotograf: " + stBild.Name + "\r\n");
        await writer.WriteLineAsync("Titel: " + stBild.Title + "\r\n");
        await writer.WriteLineAsync("Beskrivning: " + stBild.Description + "\r\n");
        await writer.WriteLineAsync("Plats: " + stBild.Location + "\r\n");
        await writer.WriteLineAsync("Datum: " + stBild.Time + "\r\n");
        await writer.WriteLineAsync("Om fotografen: " + stBild.AboutThePhotograper + "\r\n");
    }

    public static async Task CopyFileAsync(string sourcePath, string destinationPath)
    {
        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
        using (var destinationStream = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await sourceStream.CopyToAsync(destinationStream);
        }
    }
    
    public static string MakeFilenameUnique(string filename)
    {
        var uniqueFilename = filename;
        var count = 1;

        while (File.Exists(uniqueFilename))
        {
            var extension = Path.GetExtension(filename);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            uniqueFilename = $"{nameWithoutExtension} ({count}){extension}";
            count++;
        }

        return uniqueFilename;
    }
    
    public static string MakeFilenameSafe(string filename)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFilename = new StringBuilder(filename.Length);

        foreach (var c in filename)
        {
            if (invalidChars.Contains(c))
            {
                safeFilename.Append('_');
            }
            else
            {
                safeFilename.Append(c);
            }
        }

        return safeFilename.ToString();
    }
    
    public void Dispose()
    {
        if (_image == null) return;
        
        _image.Dispose();
        _image = null;
    }

    public async Task<bool> SaveThumbnailAsync()
    {
        if (_image == null)
            throw new NullReferenceException("Image is null");

        ResizeThumbnailTo165X165Size();
        
        await using (FileStream fs = new(ThumbnailPath, FileMode.Create))
        {
            await _image.SaveAsync(fs, new JpegEncoder() { Quality = 80 });
        }
        
        return true;
    }

    public FileStream? GetStreamFromRelativePath(string relativeImagePath)
    {
        var imagePath = Path.Combine(_env.ContentRootPath, ImageRootFolder, relativeImagePath);
        
        if (!File.Exists(imagePath))
            return null;

        return new FileStream(imagePath, FileMode.Open);
    }
    
    public FileStream? GetThumbnailStreamFromRelativePath(string relativeImagePath)
    {
        var thumbnailPath = Path.Combine(_env.ContentRootPath, ImageRootFolder, ConvertToThumbnailPath(relativeImagePath));
        
        if (!File.Exists(thumbnailPath))
            return null;
        
        return new FileStream(thumbnailPath, FileMode.Open);
    }
}

public interface IPhotoStore
{
    FileStream? GetStreamFromRelativePath(string relativeImagePath);
    FileStream? GetThumbnailStreamFromRelativePath(string relativeImagePath);
    Task<IPhoto?> SavePhotoAsync(Stream inputStream, (int, int) imageSize, bool resize);
    void DeletePhoto(string reativePath);
    string PhotoUserFolderAndPath { get; }

    Task PackageStBild(string imageLocalRelativeFilePath, Guid packageId, StBild stBild);
    string ZipPackage(Guid packageId, int packageNumber);
}

public interface IPhoto
{
    Task<bool> SaveThumbnailAsync();
}