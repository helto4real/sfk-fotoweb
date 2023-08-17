using FluentValidation;
using FotoApi.Abstractions;
using FotoApi.Infrastructure.Security.Authorization;

namespace FotoApi.Infrastructure.Pipelines;

/// <summary>
///     Custom pipeline that takes care of default behavior for all requests
/// </summary>
/// <remarks>
///     This behaviour is inserting current user to requests that supports ICurrentUser interface
///     and validates requests if validator is registered in DI container
/// </remarks>
public class FotoAppPipeline(CurrentUser currentUser, IServiceProvider serviceProvider) : IPipeline
{
    public async Task<TResponse> Pipe<TRequest, TResponse>(TRequest request, Func<TRequest, CancellationToken, Task<TResponse>> handler, CancellationToken ct)
    {
        await ValidateRequest(request, ct);
        // Inject the current user into the request if it implements ICurrentUser
        InjectCurrentUser(request, currentUser);
        return await handler(request, ct);
    }

    public Task<TResponse> Pipe<TResponse>(Func<CancellationToken, Task<TResponse>> handler, CancellationToken ct = default)
    {
        return handler(ct);
    }

    public async Task Pipe<TRequest>(TRequest request, Func<TRequest, CancellationToken, Task> handler, CancellationToken ct = default)
    {
        await ValidateRequest(request, ct);
        // Inject the current user into the request if it implements ICurrentUser
        InjectCurrentUser(request, currentUser);
        await handler(request, ct);
    }

    private async Task ValidateRequest<TRequest>(TRequest request, CancellationToken ct)
    {
        if (serviceProvider.GetService(typeof(IValidator<TRequest>)) is IValidator<TRequest> validator)
        {
            await validator.ValidateAndThrowAsync(request, ct);
        }
    }

    private static void InjectCurrentUser<TRequest>(TRequest request, CurrentUser currentUser)
    {
        if (request is not ICurrentUser currentUserRequest) return;
        if (currentUser.User is null)
            throw new UnauthorizedAccessException("User is not logged in");
        currentUserRequest.CurrentUser = currentUser;
    }


}