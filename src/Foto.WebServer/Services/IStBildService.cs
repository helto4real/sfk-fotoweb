using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IStBildService
{
    Task<StBildInfo?> GetStBildAsync(Guid stbildId);
    Task UpdateStBildAsync(StBildInfo stBild);
    Task<List<StBildInfo>> GetStBilder(bool showPackagedImages);
    Task <List<StBildInfo>> GetApprovedNotPackagedStBilderAsync();
    Task<bool> PackageStBilder(GuidIds guidIds);
}