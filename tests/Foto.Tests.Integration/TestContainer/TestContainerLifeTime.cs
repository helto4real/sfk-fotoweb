namespace Foto.Tests.Integration.TestContainer;

public class TestContainerLifeTime : IAsyncLifetime
{
    private TestContainer _postgressDb = new TestContainerBuilder().Build(); 
    public async Task InitializeAsync()
    {
        await _postgressDb.StartAsync();
    }

    public ushort Port => _postgressDb.Port;
    public string Host => _postgressDb.Hostname;
    public string UserName => _postgressDb.UserName;
    public string Password => _postgressDb.Password;
    
    public async Task DisposeAsync()
    {
        var (_, stderr) = await _postgressDb.GetLogsAsync();
        Console.WriteLine($"Testcontainer logs:{Environment.NewLine}{stderr}{Environment.NewLine}End of TestContainer logs");
        await _postgressDb.StopAsync();
    }
}