using System.Text.Json;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;
using FotoApi.Features.HandleSubmissions.HandleStBilder.Dto;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using Image = FotoApi.Model.Image;

namespace FotoApi.Features.HandleImages.CommandHandlers;

public class SaveImageFromStreamHandler(PhotoServiceDbContext db, IPhotoStore photoStore, NewStBildHandler handler)
    : IHandler<SaveImageFromStreamRequest, ImageResponse>
{
    private readonly ImagesMapper _mapper = new();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ImageResponse> Handle(SaveImageFromStreamRequest request, CancellationToken ct)
    {
        var metadata = GetMetadataFromMetadataType(request.MetadataType, request.Metadata);
        
        var imageSize = request.MetadataType switch
        {
            "st-bild" => (2079, 1382),
            _ => (1920, 1080)
        };
        
        var resize = request.MetadataType switch
        {
            "st-bild" => false,
            _ => true
        };
        
        var photo = await photoStore.SavePhotoAsync(request.Stream, imageSize, resize);

        if (photo is null)
            throw new FailedToSaveImageException(request.FileName);
        if (!await photo.SaveThumbnailAsync())
            throw new FailedToSaveImageException($"thumb: {request.FileName}");
        
        var photoImage = new Image
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            OwnerReference = request.CurrentUser.Id,
            LocalFilePath = photoStore.PhotoUserFolderAndPath,
            FileName = request.FileName
        };

        db.Images.Add(photoImage);
        

        await db.SaveChangesAsync(ct);

        // Todo: Send to handlers for metadata, this is kinda hacky right now, can we really have this dependency?
        if (metadata is not NewStBildRequest stBildCommand) return _mapper.ToImageResponse(photoImage);
        
        stBildCommand.OwnerReference = request.CurrentUser.Id;
        stBildCommand.ImageReference = photoImage.Id;
        await handler.Handle(stBildCommand, ct);

        return _mapper.ToImageResponse(photoImage);
    }

    private object? GetMetadataFromMetadataType(string? metadataType, string? metadata)
    {
        try
        {
            return metadataType switch
            {
                null => null,
                "st-bild" => JsonSerializer.Deserialize<NewStBildRequest>(metadata!, _jsonOptions),
                _ => throw new BadImageMetadataTypeException(metadataType)
            };
        }
        catch (Exception)
        {
            throw new BadImageMetadataFormatException(metadataType, metadata);
        }
    }
}