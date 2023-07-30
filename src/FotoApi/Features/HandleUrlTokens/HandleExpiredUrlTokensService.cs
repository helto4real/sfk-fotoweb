using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

namespace FotoApi.Features.HandleUrlTokens;

public interface IHandleExpiredUrlTokensService : IHostedService
{
};
public class HandleExpiredUrlTokensService : IHandleExpiredUrlTokensService
{
    // Remove expired tokens every 5 minutes
    private const int RemoveInterval = 5;
    private readonly PhotoServiceDbContext _db;
    private readonly ILogger<HandleExpiredUrlTokensService> _logger;
    private readonly IServiceScope _scope;
    private Task? _backgroundTask;
    private readonly CancellationTokenSource _cancelSource = new();
    private CancellationToken _combinedCancellationToken;

    public HandleExpiredUrlTokensService(IServiceProvider serviceProvider,
        ILogger<HandleExpiredUrlTokensService> logger)
    {
        _scope = serviceProvider.CreateScope();
        _logger = logger;
        var dbContext = _scope.ServiceProvider.GetService<PhotoServiceDbContext>();

        _db = dbContext ?? throw new InvalidOperationException("Failed to create db context");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _combinedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, _cancelSource.Token).Token;

        _logger.LogInformation("Starting ExpiredTokensManager");
        _backgroundTask = Task.Run(DoWork, _combinedCancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_cancelSource.IsCancellationRequested)
            await _cancelSource.CancelAsync();
        
        _scope.Dispose();
        
        _logger.LogInformation("ExpiredTokensManager - Waiting for background thread to exit");

        if (_backgroundTask is not null)
            await _backgroundTask;
    }

    private async Task DoWork()
    {
        try
        {
            while (!_combinedCancellationToken.IsCancellationRequested)
            {
                await RemoveExpiredTokens();
                await Task.Delay(TimeSpan.FromMinutes(RemoveInterval), _combinedCancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // No action needed            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in expired tokens manager");
        }
    }

    private async Task RemoveExpiredTokens()
    {
        var expiredTokens = await _db.UrlTokens.Where(token => token.ExpirationDate < DateTime.UtcNow).ToListAsync(_combinedCancellationToken);
        _db.UrlTokens.RemoveRange(expiredTokens);
        await _db.SaveChangesAsync(_combinedCancellationToken);
        if (expiredTokens.Count > 0)
            _logger.LogInformation($"Removed {expiredTokens.Count} expired tokens");
    }
}