namespace Foto.Tests;

public class EnvironmentConfigManager : IDisposable
{
    private readonly string _fotoAppConnectionString;
    private readonly string _messagesConnectionString;

    public EnvironmentConfigManager(string fotoAppConnectionString, string messagesConnectionString)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__FotoApi", fotoAppConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", messagesConnectionString);
    }
    public void Dispose()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__FotoApi", string.Empty);
        Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", String.Empty);
    }
}