using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IStBildService
{
    Task<StBildInfo?> GetStBild(Guid stbildId);
    Task UpdateStBild(StBildInfo stBild);
    Task<List<StBildInfo>> GetStBilder(bool useMyImages);
}