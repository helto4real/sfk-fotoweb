using Foto.WebServer.Dto;

namespace Foto.WebServer.Services;

public interface IMemberService
{
    Task<(MemberInfo?, ErrorDetail?)> CreateMemberAsync(NewMemberInfo newMemberInfo);
    Task<(MemberInfo?, ErrorDetail?)> UpdateMemberAsync(UpdateMemberInfo memberInfo);
    Task<(List<MemberInfo>?, ErrorDetail?)> ListMembersAsync();
    Task<(MemberInfo?, ErrorDetail?)> GetMemberByIdAsync(Guid memberId);
    Task<ErrorDetail?> DeleteMemberByIdAsync(Guid memberId);
    Task<ErrorDetail?> ActivateMemberByIdAsync(Guid memberId);
    Task<ErrorDetail?> DeactivateMemberByIdAsync(Guid memberId);
}