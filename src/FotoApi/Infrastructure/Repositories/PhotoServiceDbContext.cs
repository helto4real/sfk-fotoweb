using FotoApi.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Image = FotoApi.Model.Image;

namespace FotoApi.Infrastructure.Repositories;

public class PhotoServiceDbContext(DbContextOptions<PhotoServiceDbContext> options) :
    IdentityDbContext<User, Role, string>(options)
{
    public DbSet<UrlToken> UrlTokens => Set<UrlToken>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<StBild> StBilder => Set<StBild>();
    public DbSet<StPackage> StPackage => Set<StPackage>();
    public DbSet<StPackageItem> StPackageItem => Set<StPackageItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UrlToken>(e =>
        {
            e.HasKey(k => k.Id);
            e.Property(p => p.UrlTokenType)
                .HasConversion(
                    v => v.ToString(),
                    v => (UrlTokenType)Enum.Parse(typeof(UrlTokenType), v));
            e.Property(p => p.Token).IsRequired();
            e.Property(p => p.ExpirationDate).IsRequired();
            e.Property(p => p.UrlTokenType).IsRequired();
        });


        builder.Entity<Image>(e =>
        {
            e.HasKey(k => k.Id);
            // Do not use foreign key constraints just required fields
            e.Property(p => p.OwnerReference).IsRequired();

            e.Property(p => p.Title).IsRequired();
            e.Property(p => p.FileName).IsRequired();
            e.Property(p => p.LocalFilePath).IsRequired();
        });

        builder.Entity<StBild>(e =>
        {
            e.HasKey(k => k.Id);
            // Do not use foreign key constraints just required fields
            e.Property(p => p.OwnerReference).IsRequired();
            e.Property(p => p.ImageReference).IsRequired();
            e.Property(p => p.Title).IsRequired();
            e.Property(p => p.AboutThePhotograper).IsRequired();
            e.Property(p => p.Time).IsRequired();
            e.Property(p => p.Description).IsRequired();
            e.Property(p => p.Name).IsRequired();
        });
        builder.Entity<StPackage>(e =>
        {
            e.HasKey(k => k.Id);
            e.Property(p => p.PackageNumber).ValueGeneratedOnAdd();
            e.Property(p => p.PackageRelativPath).IsRequired();
        });
        
        builder.Entity<StPackageItem>(e =>
        {
            e.HasKey(k => k.Id);
            e.HasOne<StPackage>()
                .WithMany()
                .HasForeignKey(f => f.StPackageReference)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne<StBild>()
                .WithMany()
                .HasForeignKey(f => f.StBildReference)
                .OnDelete(DeleteBehavior.Cascade);
        });
        base.OnModelCreating(builder);
    }
}
 
