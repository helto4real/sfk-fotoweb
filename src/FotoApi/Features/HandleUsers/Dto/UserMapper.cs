using FotoApi.Features.HandleUsers.CommandHandlers;
using FotoApi.Model;
using Riok.Mapperly.Abstractions;

namespace FotoApi.Features.HandleUsers.Dto;

[Mapper]
public partial class UserMapper
{
    public UserResponse ToUserResponse(User user, bool isAdmin)
    {
        var userResponse = _ToUserResponse(user);
        userResponse.IsAdmin = isAdmin;
        return userResponse;
    }

    private partial UserResponse _ToUserResponse(User user);

    public partial UpdateUserCommand ToEditUserCommand(UserRequest request);

    public partial CreateUserRequest ToCreateUserCommand(NewUserRequest request);
}