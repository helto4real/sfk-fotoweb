using System.Text.Json.Serialization;
using BlazorStrap.Shared.InternalComponents;
using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public interface IMemberService
{
    
}

public class MemberService : IMemberService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _accessor;
    private readonly AppSettings _appSettings;
    private readonly ISignInService _signInService;
    private readonly ILogger<AdminService> _logger;

    public MemberService(HttpClient httpClient,
        IOptions<AppSettings> appSettings,
        IHttpContextAccessor accessor,
        ISignInService signInService,
        ILogger<AdminService> logger)
    {
        _httpClient = httpClient;
        _accessor = accessor;
        _appSettings = appSettings.Value;
        _signInService = signInService;
        _logger = logger;
        httpClient.BaseAddress = new Uri(_appSettings.FotoApiUrl);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FotoWebbServer");
    }
    public async Task<(MemberInfo?, ErrorDetail?)> CreateMemberAsync(NewMemberInfo newMemberInfo)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PostAsJsonAsync("api/members", newMemberInfo));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var member = await response.Content.ReadFromJsonAsync<MemberInfo>();
        return (member, null);
    }
    
    public async Task<(List<MemberInfo>?, ErrorDetail?)> ListMembersAsync()
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync("api/members"));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var members = await response.Content.ReadFromJsonAsync<List<MemberInfo>>();
        return (members, null);
    }
    
    public async Task<(MemberInfo? member, ErrorDetail? error)> GetMemberByIdAsync(Guid memberId)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"api/members/{memberId}"));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var member = await response.Content.ReadFromJsonAsync<MemberInfo>();
        return (member, null);
    }

    public async Task<ErrorDetail?> DeleteMemberByIdAsync(Guid memberId)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.DeleteAsync($"api/members/{memberId}"));
        return await HandleResponse(response);
    }

    public async Task<ErrorDetail?> ActivateMemberByIdAsync(Guid memberId)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"api/members/{memberId}/activate"));
        return await HandleResponse(response);
    }

    public async Task<ErrorDetail?> DeactivateMemberByIdAsync(Guid memberId)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.GetAsync($"api/members/{memberId}/deactivate"));
        return await HandleResponse(response);
    }

    private async Task<ErrorDetail?> HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<ErrorDetail>();

        return null;
    }

}