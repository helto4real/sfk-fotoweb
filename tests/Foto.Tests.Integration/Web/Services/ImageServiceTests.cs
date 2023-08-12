using FluentAssertions;
using Foto.Tests.Integration.TestContainer;
using Foto.WebServer.Dto;
using Foto.WebServer.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Foto.Tests.Integration.Web.Services;

[Collection("Integration tests collection")]
public class ImageServiceTests: IntegrationTestsBase
{
    [Fact]
    public async Task UploadImageWithMetadataShouldSucceed()
    {
        // Arrange
        var client = CreateClient("admin", true);
        var options = Options.Create(new AppSettings() { FotoApiUrl = client.BaseAddress!.ToString() });
        var fileName = "somefile.jpg";
        var db = CreateFotoAppDbContext();
        var stBildMetaData = new StBildMetadata()
        {
            Title = "Title",
            Name = "Name",
            Location = "Location",
            Time = new DateTime(2023, 1, 1).ToUniversalTime(),
            Description = "Description",
            AboutThePhotographer = "AboutThePhotographer"
        };
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream);
        
        var fileMock = FileMock(writer, fileName, memoryStream);
        // Act
        var imageService = new ImageService(client, options, new FakeSignInService(), new Mock<ILogger<ImageService>>().Object);
        var (image, error) = await imageService.UploadImageWithMetadata(fileMock.Object, "Title", stBildMetaData, "st-bild");
        
        // Assert
        error.Should().BeNull();
        image.Should().NotBeNull();
        App.PhotoStoreMock.Verify(e => e.SavePhotoAsync(It.IsAny<Stream>(), It.IsAny<(int, int)>(), It.IsAny<bool>()), Times.Once());
        image!.Title.Should().Be("Title");
        var dbImage = await db.Images.FindAsync(image.Id);
        dbImage.Should().NotBeNull();
        dbImage!.Title.Should().Be("Title");
        var stBild = db.StBilder.Single(n => n.ImageReference == image.Id);
        stBild.Description.Should().Be("Description");
        stBild.AboutThePhotographer.Should().Be("AboutThePhotographer");
        stBild.Location.Should().Be("Location");
        stBild.Name.Should().Be("Name");
        stBild.Time.Should().Be(new DateTime(2023, 1, 1).ToUniversalTime());
    }

    private static Mock<IBrowserFile> FileMock(StreamWriter writer, string fileName, MemoryStream memoryStream)
    {
        var content = "Hello World!";
        writer.Write(content); // This is not a real file since we are mocking

        var fileMock = new Mock<IBrowserFile>();
        fileMock.Setup(f => f.Name).Returns(fileName);
        fileMock.Setup(f => f.Size).Returns(memoryStream.Length);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        fileMock.Setup(f => f.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>())).Returns(memoryStream);
        return fileMock;
    }

    public ImageServiceTests(TestContainerLifeTime testContinerLifetime) : base(testContinerLifetime)
    {
    }
}

public class StBildMetadata
{
    public string Title { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime Time { get; set; } = new DateTime(2023, 1, 1);
    public string Description { get; set; } = default!;
    public string AboutThePhotographer { get; set; } = default!;

}
