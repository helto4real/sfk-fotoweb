using FotoApi.Infrastructure.Validation.Exceptions;

namespace FotoApi.Features.HandleUsers.Exceptions;

public sealed class FailedToSetNewPasswordException(string user) : 
    BadRequestException($"Misslyckades att sätta nytt lösenord för användare  {user}.");