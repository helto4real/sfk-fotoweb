using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class FailedToSetNewPasswordException : BadRequestException
{
    public FailedToSetNewPasswordException(string user)
        : base($"Misslyckades att sätta nytt lösenprd för användare  {user}.")
    {
    }
}