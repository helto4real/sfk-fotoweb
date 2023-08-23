using FotoApi.Abstractions;
using FotoApi.Api;
using FotoApi.Infrastructure.Repositories;
using FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;
using FotoApi.Infrastructure.Security.Authorization;
using FotoApi.Model;
using Microsoft.AspNetCore.SignalR;

namespace FotoApi.Features.HandleSubmissions.HandleStBilder.Commands;

public record PackageStBilderRequest(IReadOnlyCollection<Guid> StBildIds) : ICurrentUser
{
    public CurrentUser CurrentUser { get; set; } = default!;
}

public class PackageStBilderHandler(
        IHubContext<SignalRApi> ctx,
        IPhotoStore photoStore,
        IServiceProvider serviceProvider,
        ILogger<PackageStBilderHandler> logger)
    : IHandler<PackageStBilderRequest>
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public Task Handle(PackageStBilderRequest request, CancellationToken ct)
    {
        if (SemaphoreSlim.CurrentCount == 0)
            // We do not want to run this command more than once at a time
            return Task.CompletedTask;
        var newScope = serviceProvider.CreateScope();
        _ = PackageStBilder(request.StBildIds, request.CurrentUser, newScope);
        return Task.CompletedTask;
    }

    public async Task PackageStBilder(IReadOnlyCollection<Guid> stBildIds, CurrentUser owner, IServiceScope scope)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<PhotoServiceDbContext>();
            var stBilderThatArePackable = await db.StBilder.Where(e => e.IsAccepted && !e.IsUsed).ToListAsync();
            var packageId = Guid.NewGuid();

            var nextPackageNumber = 1;
            var imagesToPackage = stBilderThatArePackable.Where(e => stBildIds.Contains(e.Id)).ToList();
            if (await db.StPackage.AnyAsync())
                nextPackageNumber = await db.StPackage.MaxAsync(e => e.PackageNumber) + 1;

            var nrOfImagesToPackage = imagesToPackage.Count;
            var nrOfImagesPackaged = 1;
            foreach (var stBild in imagesToPackage)
            {
                var progress = Math.Round((double)75 / (nrOfImagesToPackage - nrOfImagesPackaged));
                await ctx.Clients.User(owner.User!.UserName!).SendAsync("package_progress", (int)progress);
                var image = db.Images.SingleOrDefault(e => e.Id == stBild.ImageReference);
                if (image == null)
                    // Just ignore errors for now. We should probably log this
                    continue;
                await photoStore.PackageStBild(image.LocalFilePath, packageId, stBild);
                stBild.IsUsed = true;
                nrOfImagesPackaged++;
            }

            var stPackage = await db.StPackage.AddAsync(
                new StPackage
                {
                    Id = packageId,
                    PackageRelativPath = "temppath",
                    IsDelivered = false,
                    PackageNumber = nextPackageNumber
                }
            );

            var zipFilePath = photoStore.ZipPackage(packageId, stPackage.Entity.PackageNumber);
            foreach (var stBild in imagesToPackage)
                await db.StPackageItem.AddAsync(
                    new StPackageItem
                    {
                        StBildReference = stBild.Id,
                        StPackageReference = packageId
                    });
            stPackage.Entity.PackageRelativPath = zipFilePath;
            await db.SaveChangesAsync();
            await ctx.Clients.User(owner.User!.UserName!).SendAsync("package_progress", 100);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while packaging stBilder");
            throw;
        }
        finally
        {
            scope.Dispose();
            SemaphoreSlim.Release();
        }
    }
}