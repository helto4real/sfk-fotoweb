﻿using FotoApi.Api;

namespace FotoApi.Features.HandleMembers.Dto;

public record MemberResponse
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public List<RoleResponse> Roles { get; set; } = new(); 
    public bool IsActive { get; set; }
}

