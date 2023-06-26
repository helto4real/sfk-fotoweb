using FotoApi.Features.HandleUsers.Commands;
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

    public partial CreateUserCommand ToCreateUserCommand(NewUserRequest request);
}