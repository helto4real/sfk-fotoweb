﻿using Foto.WebServer.Dto;
using Microsoft.Extensions.Options;

namespace Foto.WebServer.Services;

public class MemberService : ServiceBase, IMemberService
{
    private readonly HttpClient _httpClient;
    private readonly ISignInService _signInService;

    public MemberService(HttpClient httpClient,
        IOptions<AppSettings> appSettings,
        ISignInService signInService,
        ILogger<MemberService> logger) : base(logger)
    {
        _httpClient = httpClient;
        _signInService = signInService;
        httpClient.BaseAddress = new Uri(appSettings.Value.FotoApiUrl);
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

    public async Task<(MemberInfo?, ErrorDetail?)> GetMemberByIdAsync(Guid memberId)
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

    public async Task<(MemberInfo?, ErrorDetail?)> UpdateMemberAsync(UpdateMemberInfo memberInfo)
    {
        var response = await _signInService.RefreshTokenOnExpired(async () =>
            await _httpClient.PutAsJsonAsync("api/members", memberInfo));
        var result = await HandleResponse(response);
        if (result is not null) return (null, result);
        var member = await response.Content.ReadFromJsonAsync<MemberInfo>();
        return (member, null);
    }
}