using FotoApi.Features.HandleStBilder.Commands;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleStBilder.Dto;

[Mapper]
public partial class StBildMapper
{
    public NewStBildCommand ToNewStBildCommand(StBildRequest request, CurrentUser owner, Guid imageId)
    {
        var stBildCommand = _ToNewStBildCommand(request);
        stBildCommand.OwnerReference = owner.Id;
        stBildCommand.ImageReference = imageId;
        return stBildCommand;
    }

    public partial StBildResponse ToStBildResponse(StBild stBild);
    public partial StBild ToStBild(NewStBildCommand command);
    public partial StBild ToStBild(UpdateStBildCommand command);
    private partial UpdateStBildCommand _ToStBildCommand(StBildRequest command);

    public UpdateStBildCommand ToStBildCommand(StBildRequest command, Guid id)
    {
        var stBildCommand = _ToStBildCommand(command);
        stBildCommand.Id = id;
        return stBildCommand;
    }
    
    private partial NewStBildCommand _ToNewStBildCommand(StBildRequest command);
}