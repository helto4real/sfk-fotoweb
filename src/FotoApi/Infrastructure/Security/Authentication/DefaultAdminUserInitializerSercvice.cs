using FotoApi.Model;
using Microsoft.AspNetCore.Identity;

namespace FotoApi.Infrastructure.Security.Authentication;

public class DefaultAdminUserInitializerSercvice : IHostedService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IServiceScope _scope;
    
    public DefaultAdminUserInitializerSercvice(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateScope();
        _userManager = _scope.ServiceProvider.GetService<UserManager<User>>() ?? throw new InvalidOperationException("Failed to create user manager");
        _roleManager = _scope.ServiceProvider.GetService<RoleManager<Role>>() ?? throw new InvalidOperationException("Failed to create user manager");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Create roles
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new Role("Admin"));
        }

        // Create users
        if (await _userManager.FindByNameAsync("admin") == null)
        {
            var user = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
            };

            var result = await _userManager.CreateAsync(user, "P@ssw0rd!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }
}

