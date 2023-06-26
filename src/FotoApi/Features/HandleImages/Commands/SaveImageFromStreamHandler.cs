using System.Text.Json;
using FotoApi.Features.HandleImages.Dto;
using FotoApi.Features.HandleImages.Exceptions;
using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Validation.Exceptions;
using System.Net.Http.Json;
using MediatR;
using Image = FotoApi.Model.Image;

namespace FotoApi.Features.HandleImages.Commands;

public class SaveImageFromStreamHandler : ICommandHandler<SaveImageFromStreamCommand, ImageResponse>
{
    private readonly PhotoServiceDbContext _db;
    private readonly ImagesMapper _mapper = new();
    private readonly IPhotoStore _photoStore;
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public SaveImageFromStreamHandler(PhotoServiceDbContext db, IPhotoStore photoStore, IMediator mediator)
    {
        _db = db;
        _photoStore = photoStore;
        _mediator = mediator;
    }

    public async Task<ImageResponse> Handle(SaveImageFromStreamCommand command, CancellationToken cancellationToken)
    {
        if (command.Owner.User is null)
            throw new ForbiddenException("You must be logged in to save images");

        var metadata = GetMetadataFromMetadataType(command.MetadataType, command.Metadata);
        
        var imageSize = command.MetadataType switch
        {
            "st-bild" => (2079, 1382),
            _ => (1920, 1080)
        };
        
        var resize = command.MetadataType switch
        {
            "st-bild" => false,
            _ => true
        };
        
        var photo = await _photoStore.SavePhotoAsync(command.Stream, imageSize, resize);

        if (photo is null)
            throw new FailedToSaveImageException(command.FileName);
        if (!await photo.SaveThumbnailAsync())
            throw new FailedToSaveImageException($"thumb: {command.FileName}");
        
        var photoImage = new Image
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            OwnerReference = command.Owner.Id,
            LocalFilePath = _photoStore.PhotoUserFolderAndPath,
            FileName = command.FileName
        };

        _db.Images.Add(photoImage);
        

        await _db.SaveChangesAsync();

        
        
        // Todo: Send to handlers for metadata
        if (metadata is NewStBildCommand stBildCommand)
        {
            stBildCommand.OwnerReference = command.Owner.Id;
            stBildCommand.ImageReference = photoImage.Id;
            await _mediator.Send(stBildCommand);
        }
        
        
        
        return _mapper.ToImageResponse(photoImage);
    }

    private object? GetMetadataFromMetadataType(string? metadataType, string? metadata)
    {
        try
        {
            return metadataType switch
            {
                null => null,
                "st-bild" => JsonSerializer.Deserialize<NewStBildCommand>(metadata!, _jsonOptions),
                _ => throw new BadImageMetadataTypeException(metadataType)
            };
        }
        catch (Exception)
        {
            throw new BadImageMetadataFormatException(metadataType, metadata);
        }
    }
}