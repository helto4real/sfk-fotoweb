using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IStBildService
{
    Task<StBildInfo?> GetStBildAsync(Guid stbildId);
    Task UpdateStBildAsync(StBildInfo stBild);
    Task<List<StBildInfo>> GetStBilder(bool showPackagedImages);
    Task<List<StBildInfo>> GetStBilderForCurrentUser(bool showPackagedImages);
    Task<List<StBildInfo>> GetApprovedNotPackagedStBilderAsync();
    Task<bool> PackageStBilder(GuidIds guidIds);
    Task<ErrorDetail?> SetAcceptStatusForStBild(Guid stBildId, bool stBildIsAccepted);
    Task<(List<StBildPackageInfo>?, ErrorDetail?)> GetStBildPackagesAsync(bool returnDelivered);
    Task<ErrorDetail?> SetPackageStatusDelivered(Guid id);
}