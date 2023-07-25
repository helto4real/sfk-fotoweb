using FotoApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Image = FotoApi.Model.Image;

namespace FotoApi.Infrastructure.Repositories.PhotoServiceDbContext;

public class PhotoServiceDbContext: IdentityDbContext<User, Role, string>
{
    public PhotoServiceDbContext(DbContextOptions<PhotoServiceDbContext> options) : base(options)
    {
        
    }
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
        
        //Seed initial values
        var adminRoleId = Guid.NewGuid().ToString();
        var adminId = Guid.NewGuid().ToString();
        
        var defaultAdminUser = new User
        {
            Id = adminId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@localhost",
            NormalizedEmail = "ADMIN@LOCALHOST",
            RefreshToken = "Some non existing token",
            RefreshTokenExpirationDate = DateTime.MinValue
        };
        defaultAdminUser.PasswordHash = new PasswordHasher<User>().HashPassword(defaultAdminUser, "P@ssw0rd!");
        
        builder.Entity<User>().HasData(defaultAdminUser);
        
        builder.Entity<Role>()
            .HasData(
                new Role
                { 
                    Name = "Admin",
                    NormalizedName = "ADMIN", 
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                });
        
        
        
        builder.Entity<IdentityUserRole<string>>()
            .HasData(
                new IdentityUserRole<string> 
                { 
                    RoleId = adminRoleId, 
                    UserId = adminId
                });
            
        base.OnModelCreating(builder);
    }
    
    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is TimeTrackedEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        var utcTimeNow = DateTime.UtcNow;
        foreach (var entityEntry in entries)
        {
            ((TimeTrackedEntity)entityEntry.Entity).UpdatedDate = utcTimeNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((TimeTrackedEntity)entityEntry.Entity).CreatedDate = utcTimeNow;
            }
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is TimeTrackedEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        var utcTimeNow = DateTime.UtcNow;
        foreach (var entityEntry in entries)
        {
            ((TimeTrackedEntity)entityEntry.Entity).UpdatedDate = utcTimeNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((TimeTrackedEntity)entityEntry.Entity).CreatedDate = utcTimeNow;
            }
        }
        
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
 
