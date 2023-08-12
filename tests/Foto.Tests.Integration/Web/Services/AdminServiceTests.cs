using FluentAssertions;
using Foto.Tests.Integration.TestContainer;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Foto.Tests.Integration.Web.Services;


[Collection("Integration tests collection")]
public class AdminServiceTests : IntegrationTestsBase
{
    [Fact]
    public async Task GetRolesShouldReturnRoles()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        
        var adminService = new AdminService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
            );

        // Act
        var (roles, error) = await adminService.GetRolesAsync();

        // Assert
        roles.Should().NotBeNull();
        error.Should().BeNull();
        roles.Should().Contain(e => e.Name == "Admin");
        roles.Should().Contain(e => e.Name == "Member");
        roles.Should().Contain(e => e.Name == "CompetitionAdministrator");
        roles.Should().Contain(e => e.Name == "StbildAdministrator");
        
    }
    public AdminServiceTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}