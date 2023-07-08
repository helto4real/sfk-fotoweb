using FotoApi.Api;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace FotoApi.Features.HandleStBilder.Commands;

public class PackageStBilderHandler : ICommandHandler<PackageStBilderCommand, bool>
{
    private readonly IHubContext<SignalRApi> _ctx;
    private readonly IPhotoStore _photoStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PackageStBilderHandler> _logger;
    private static SemaphoreSlim _semaphoreSlim = new(1, 1);
    public PackageStBilderHandler(
        IHubContext<SignalRApi> ctx, 
        IPhotoStore photoStore,
        IServiceProvider serviceProvider,
        ILogger<PackageStBilderHandler> logger)
    {
        _ctx = ctx;
        _photoStore = photoStore;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public Task<bool> Handle(PackageStBilderCommand request, CancellationToken cancellationToken)
    {
        if (_semaphoreSlim.CurrentCount == 0)
        {
            // We do not want to run this command more than once at a time
            return Task.FromResult(false);
        }

        var newScope = _serviceProvider.CreateScope();
        _ = PackageStBilder(request.StBildIds, request.Owner, newScope);
        return Task.FromResult(true);
    }
    
    public async Task PackageStBilder(List<Guid> stBildIds, CurrentUser owner, IServiceScope scope)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<PhotoServiceDbContext>();
            var stBilderThatArePackageable = await db.StBilder.Where(e => e.IsAccepted && !e.IsUsed).ToListAsync();
            var packageId = Guid.NewGuid();

            var nextPackageNumber = 1;
            var imagesToPackage = stBilderThatArePackageable.Where(e => stBildIds.Contains(e.Id)).ToList();
            if (await db.StPackage.AnyAsync())
            {
                nextPackageNumber = await db.StPackage.MaxAsync(e => e.PackageNumber) + 1;
            }
            
            var nrOfImagesToPackage = imagesToPackage.Count;
            var nrOfImagesPackaged = 1;
            foreach (var stBild in imagesToPackage)
            {
                double progress = Math.Round((double)75 / (nrOfImagesToPackage - nrOfImagesPackaged));
                await _ctx.Clients.User(owner.User!.UserName!).SendAsync("package_progress", (int) progress);
                var image = db.Images.SingleOrDefault(e => e.Id == stBild.ImageReference);
                if (image == null)
                {
                    // Just ignore errors for now. We should probably log this
                    continue;
                }
                await _photoStore.PackageStBild(image.LocalFilePath, packageId, stBild);
                stBild.IsUsed = true;
                nrOfImagesPackaged++;
            }

            var stPackage = await db.StPackage.AddAsync(
                new StPackage()
                {
                    Id = packageId,
                    PackageRelativPath = "temppath",
                    IsDelivered = false,
                    PackageNumber = nextPackageNumber
                }
            );
            
            var zipFilePath = _photoStore.ZipPackage(packageId, stPackage.Entity.PackageNumber);
            foreach (var stBild in imagesToPackage)
            {
                await db.StPackageItem.AddAsync(
                    new StPackageItem()
                    {
                        StBildReference = stBild.Id,
                        StPackageReference = packageId
                    });
            }
            stPackage.Entity.PackageRelativPath = zipFilePath;
            await db.SaveChangesAsync();
            await _ctx.Clients.User(owner.User!.UserName!).SendAsync("package_progress", 100);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while packaging stBilder");
            throw;
        }
        finally
        {
            scope.Dispose();
            _semaphoreSlim.Release();
        }
    }
}