using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace Foto.Tests.Integration.TestContainer;

public class TestContainer : DockerContainer
{
    private readonly TestContainerConfiguration _configuration;
    public ushort Port => GetMappedPublicPort(5432);
    
    public string UserName => _configuration.Username;
    public string Password => _configuration.Password;
    public TestContainer(TestContainerConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        _configuration = configuration;
    }
}