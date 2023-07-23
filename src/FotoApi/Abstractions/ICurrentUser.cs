using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Abstractions;

public interface ICurrentUser
{
    public CurrentUser CurrentUser { get; set; }
}