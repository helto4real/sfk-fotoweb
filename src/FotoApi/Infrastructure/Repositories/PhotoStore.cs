using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Infrastructure.Security.Authorization;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace FotoApi.Infrastructure.Repositories;

/// <summary>
///     Manage the storage of photos on file system
/// </summary>
public class PhotoStore : IPhotoStore, IPhoto, IDisposable
{
    private readonly IWebHostEnvironment _env;
    private readonly CurrentUser? _currentUser;
    private Image? _image;
    private readonly string _photoUserFolderAndPath;
    private readonly string _imagePath;

    private const string ImageRootFolder = ".images/user_images";
    
    public PhotoStore(IWebHostEnvironment env, CurrentUser currentUser)
    {
        _env = env;
        _currentUser = currentUser;
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
            ResizeImage(width, height);
        else
        {
            if (_image.Width != width || _image.Height != height)
                throw new WrongImageSizeException(width, height);
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

}

public interface IPhoto
{
    Task<bool> SaveThumbnailAsync();
}