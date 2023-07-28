using FluentAssertions;
using Foto.Tests.Integration.TestContainer;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Foto.Tests.Integration.Web.Services;

[Collection("Integration tests collection")]
public class MemberServiceTests : IntegrationTestsBase
{
    [Fact]
    public async Task CreateMemberShouldReturnCreatedUser()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        
        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: "somemember@domain.com",
            PhoneNumber: "12345678",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999"));
        
        // Assert
        error.Should().BeNull();
        member.Should().NotBeNull();
        member!.FirstName.Should().Be("MemberName");
        member.LastName.Should().Be("MemberLastName");
        member.Email.Should().Be("somemember@domain.com");
        member.Address.Should().Be("MemberStreet 1");
        member.ZipCode.Should().Be("9999");
        member.PhoneNumber.Should().Be("12345678");
        member.IsActive.Should().BeTrue();
        member.UserName.Should().Be("somemember@domain.com");
    }    
    
    [Fact]
    public async Task CreateMemberShouldReturnCreatedUserForExistingUser()
    {
        // Arrange
        var existingUserEmail = "existing_user_member@domain.com";
        
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );

        await CreateUserAsync(existingUserEmail, null, existingUserEmail);
        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: existingUserEmail,
            PhoneNumber: "12345678",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999"));
        
        // Assert
        error.Should().BeNull();
        member.Should().NotBeNull();
        member!.FirstName.Should().Be("MemberName");
        member.LastName.Should().Be("MemberLastName");
        member.Email.Should().Be(existingUserEmail);
        member.Address.Should().Be("MemberStreet 1");
        member.ZipCode.Should().Be("9999");
        member.PhoneNumber.Should().Be("12345678");
        member.IsActive.Should().BeTrue();
        member.UserName.Should().Be(existingUserEmail);
    }

    [Fact]
    public async Task CreateMemberShouldReturnErrorWhenMemberAlreadyExists()
    {
        // Arrange
        var existingMemberEmail = "existingmember@somedomain.com";
        var client = CreateClient("admin", true);
        await CreateMember(existingMemberEmail);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        
        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: existingMemberEmail,
            PhoneNumber: "12345678",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999"));
        
        // Assert
        error.Should().NotBeNull();
        member.Should().BeNull();
        
        error!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ListMembersShouldReturnResults()
    {
        // Arrange
        var client = CreateClient("admin", true);
        await CreateMember("listmember1@domain.com");
        await CreateMember("listmember2@domain.com");
        await CreateMember("listmember3@domain.com");
        
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        
        // Act
        var (members, error) = await memberService.ListMembersAsync();
        
        // Assert
        error.Should().BeNull();
        members.Should().NotBeNull();
        members!.Where(x => x.Email.StartsWith("listmember")).ToList().Count.Should().Be(3);
    }

    [Fact]
    public async Task GetMemberByIdShouldReturnMember()
    {
        // Arrange
        var existingMemberEmail = "existingmember@domain.com";
        var client = CreateClient("admin", true);
        var existingMember = await CreateMember(existingMemberEmail);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var (member, error) = await memberService.GetMemberByIdAsync(existingMember.Id);
        
        // Assert
        error.Should().BeNull();
        member.Should().NotBeNull();
        member!.Id.Should().Be(existingMember.Id);
        member.Email.Should().Be(existingMemberEmail);
        member.Address.Should().Be("MemberStreet 1");
        member.ZipCode.Should().Be("9999");
        member.FirstName.Should().Be("MemberFirstName");
        member.LastName.Should().Be("MemberLastName");
        member.IsActive.Should().BeTrue();
        member.UserName.Should().Be(existingMemberEmail);
    }

    [Fact]
    public async Task GetNonExistingMemberByIdShouldReturnError()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var nonExistingMemberId = Guid.NewGuid();
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var (member, error) = await memberService.GetMemberByIdAsync(nonExistingMemberId);
        
        // Assert
        error.Should().NotBeNull();
        member.Should().BeNull();
        error!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task DeleteMemberShouldReturnSuccess()
    {
        // Arrange
        var existingMemberEmail = "membertobedeleted@domain.com";
        var client = CreateClient("admin", true);
        var existingMember = await CreateMember(existingMemberEmail);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var error = await memberService.DeleteMemberByIdAsync(existingMember.Id);
        
        // Assert
        error.Should().BeNull();
    }        
    
    [Fact]
    public async Task ActivateMemberShouldReturnSuccess()
    {
        // Arrange
        var activeMemberEmail = "active_member@domain.com";
        var client = CreateClient("admin", true);
        var existingMember = await CreateMember(activeMemberEmail, false);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var error = await memberService.ActivateMemberByIdAsync(existingMember.Id);
        
        // Assert
        error.Should().BeNull();
        var deactivatedMember = await db.Members.FindAsync(existingMember.Id);
        deactivatedMember!.IsActive.Should().BeTrue();
    }    

    [Fact]
    public async Task DeactivateMemberShouldReturnSuccess()
    {
        // Arrange
        var inactiveMemberEmail = "inactive_member@domain.com";
        var client = CreateClient("admin", true);
        var existingMember = await CreateMember(inactiveMemberEmail);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var error = await memberService.DeactivateMemberByIdAsync(existingMember.Id);
        
        // Assert
        error.Should().BeNull();
        var activatedMember = await db.Members.FindAsync(existingMember.Id);
        activatedMember!.IsActive.Should().BeFalse();
    }    
    
    [Fact]
    public async Task DeactivateNonExistingMemberShouldReturnError()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var nonExistingMemberId = Guid.NewGuid();
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var error = await memberService.DeactivateMemberByIdAsync(nonExistingMemberId);
        
        // Assert
        error.Should().NotBeNull();
        error!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    } 
    
    [Fact]
    public async Task DeleteNonExistingMemberShouldReturnError()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var nonExistingMemberId = Guid.NewGuid();
        var memberService = new MemberService(
            client,
            options,
            new Mock<IHttpContextAccessor>().Object,
            new FakeSignInService(),
            new Mock<ILogger<AdminService>>().Object
        );
        // Act
        var error = await memberService.DeleteMemberByIdAsync(nonExistingMemberId);
        
        // Assert
        error.Should().NotBeNull();
        error!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
    
    public MemberServiceTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}