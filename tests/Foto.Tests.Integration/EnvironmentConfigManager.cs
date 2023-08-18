namespace Foto.Tests.Integration;

public class EnvironmentConfigManager : IDisposable
{
    public EnvironmentConfigManager(string fotoAppConnectionString, string messagesConnectionString)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__FotoApi", fotoAppConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", messagesConnectionString);
    }
    public void Dispose()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__FotoApi", string.Empty);
        Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", string.Empty);
    }
}