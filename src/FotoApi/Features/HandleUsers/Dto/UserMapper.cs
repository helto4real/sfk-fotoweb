using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleUsers.Dto;

[Mapper]
public partial class UserMapper
{
    public UserResponse ToUserResponse(User user, IReadOnlyCollection<string> roles)
    {
        var userResponse = _ToUserResponse(user);
        userResponse.Roles = roles;
        return userResponse;
    }

    private partial UserResponse _ToUserResponse(User user);

    public partial UpdateUserRequest ToEditUserCommand(UserRequest request);

    public partial CreateUserRequest ToCreateUserCommand(NewUserRequest request);
}