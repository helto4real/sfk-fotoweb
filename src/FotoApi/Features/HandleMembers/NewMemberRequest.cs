namespace FotoApi.Features.HandleMembers;

public record NewMemberRequest
{
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
}