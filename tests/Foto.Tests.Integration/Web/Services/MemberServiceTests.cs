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
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: "somemember@domain.com",
            PhoneNumber: "070-123456",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999",
            City: "MemberCity",
            Roles: new List<RoleInfo> { new() { Name = "Member" } }));

        // Assert
        error.Should().BeNull();
        member.Should().NotBeNull();
        member!.FirstName.Should().Be("MemberName");
        member.LastName.Should().Be("MemberLastName");
        member.Email.Should().Be("somemember@domain.com");
        member.Address.Should().Be("MemberStreet 1");
        member.ZipCode.Should().Be("9999");
        member.City.Should().Be("MemberCity");
        member.PhoneNumber.Should().Be("070-123456");
        member.IsActive.Should().BeTrue();
        member.UserName.Should().Be("somemember@domain.com");
    }

    [Fact]
    public async Task CreateMemberShouldReturnCreatedUserForExistingUser()
    {
        // Arrange
        var existingUserEmail = "existing_user_member@domain.com";

        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        await CreateUserAsync(existingUserEmail, null, existingUserEmail);
        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: existingUserEmail,
            PhoneNumber: "08123456",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999",
            City: "MemberCity",
            Roles: new List<RoleInfo> { new() { Name = "Member" } }));


        // Assert
        error.Should().BeNull();
        member.Should().NotBeNull();
        member!.FirstName.Should().Be("MemberName");
        member.LastName.Should().Be("MemberLastName");
        member.Email.Should().Be(existingUserEmail);
        member.Address.Should().Be("MemberStreet 1");
        member.ZipCode.Should().Be("9999");
        member.City.Should().Be("MemberCity");
        member.PhoneNumber.Should().Be("08123456");
        member.IsActive.Should().BeTrue();
        member.UserName.Should().Be(existingUserEmail);
        member.Roles.Should().HaveCount(1);
        member.Roles.First().Name.Should().Be("Member");
    }

    [Fact]
    public async Task UpdateMemberShouldReturnUpdatedUser()
    {
        // Arrange
        var updatedMemberEmail = "updated_user_member@domain.com";

        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        var member = await CreateMemberAsync(updatedMemberEmail);

        // Act
        var (updatedMember, error) = await memberService.UpdateMemberAsync(new UpdateMemberInfo(
            member.Id,
            "new_updated_email@domain.com",
            "070-7654321",
            "MemberNameUpdated",
            "MemberLastNameUpdated",
            "MemberStreet 2",
            "123456",
            "MemberCityUpdated",
            new List<RoleInfo> { new() { Name = "Admin" } }
        ));

        // Assert
        error.Should().BeNull();
        updatedMember.Should().NotBeNull();
        updatedMember!.FirstName.Should().Be("MemberNameUpdated");
        updatedMember.LastName.Should().Be("MemberLastNameUpdated");
        updatedMember.Email.Should().Be("new_updated_email@domain.com");
        updatedMember.Address.Should().Be("MemberStreet 2");
        updatedMember.ZipCode.Should().Be("123456");
        updatedMember.City.Should().Be("MemberCityUpdated");
        updatedMember.PhoneNumber.Should().Be("070-7654321");
        updatedMember.IsActive.Should().BeTrue();
        updatedMember.UserName.Should().Be(updatedMemberEmail);
        updatedMember.Roles.Should().HaveCount(1);
        updatedMember.Roles.First().Name.Should().Be("Admin");
    }

    [Fact]
    public async Task CreateMemberShouldReturnErrorWhenMemberAlreadyExists()
    {
        // Arrange
        var existingMemberEmail = "existingmember@somedomain.com";
        var client = CreateClient("admin", true);
        await CreateMemberAsync(existingMemberEmail);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        // Act
        var (member, error) = await memberService.CreateMemberAsync(new NewMemberInfo(
            Email: existingMemberEmail,
            PhoneNumber: "12345678",
            FirstName: "MemberName",
            LastName: "MemberLastName",
            Address: "MemberStreet 1",
            ZipCode: "9999",
            City: "MemberCity",
            Roles: new List<RoleInfo> { new() { Name = "Member" } }));

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
        await CreateMemberAsync("listmember1@domain.com");
        await CreateMemberAsync("listmember2@domain.com");
        await CreateMemberAsync("listmember3@domain.com");

        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var existingMember = await CreateMemberAsync(existingMemberEmail);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        member.City.Should().Be("MemberCity");
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
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var nonExistingMemberId = Guid.NewGuid();

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var existingMember = await CreateMemberAsync(existingMemberEmail);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var existingMember = await CreateMemberAsync(activeMemberEmail, false);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var existingMember = await CreateMemberAsync(inactiveMemberEmail);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });

        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
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
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var nonExistingMemberId = Guid.NewGuid();
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        // Act
        var error = await memberService.DeleteMemberByIdAsync(nonExistingMemberId);

        // Assert
        error.Should().NotBeNull();
        error!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task ActivateMemberStateShouldReturnSuccess()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();
        var member = await CreateMemberAsync("inactive_member@anydomain.com", false, new List<string> { "Member" });
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        // Act
        var error = await memberService.ActivateMemberByIdAsync(member.Id);

        // Assert
        error.Should().BeNull();
        (await db.Members.FindAsync(member.Id))!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateMemberStateShouldReturnSuccess()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings { FotoApiUrl = client.BaseAddress!.ToString() });
        var db = CreateFotoAppDbContext();
        var member = await CreateMemberAsync("active_member@anydomain.com", true, new List<string> { "Member" });
        var memberService = new MemberService(
            client,
            options,
            new FakeSignInService(),
            new Mock<ILogger<MemberService>>().Object
        );

        // Act
        var error = await memberService.DeactivateMemberByIdAsync(member.Id);

        // Assert
        error.Should().BeNull();
        (await db.Members.FindAsync(member.Id))!.IsActive.Should().BeFalse();
    }

    public MemberServiceTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}