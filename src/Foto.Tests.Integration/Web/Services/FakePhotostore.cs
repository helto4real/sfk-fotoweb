using FotoApi.Infrastructure.Repositories;
using Moq;

namespace Foto.Tests.Integration.Web.Services;

public class FakePhotostore : Mock<IPhotoStore>
{
    public FakePhotostore()
    {
        var fakeIPhoto = new Mock<IPhoto>();
        fakeIPhoto.Setup(x => x.SaveThumbnailAsync()).ReturnsAsync(true);
        
        Setup(x => x.PhotoUserFolderAndPath).Returns("C:\\user-path");
        Setup(x => x.SavePhotoAsync(It.IsAny<Stream>(), It.IsAny<(int, int)>(), It.IsAny<bool>())).ReturnsAsync(fakeIPhoto.Object);
    }
}