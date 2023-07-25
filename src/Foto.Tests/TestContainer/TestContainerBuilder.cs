using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Foto.Tests.TestContainer;

public class TestContainerBuilder : ContainerBuilder<TestContainerBuilder, TestContainer, TestContainerConfiguration>
{
    private const string DefaultVersion = "15.3-alpine";
    private const string DefaultUsername = "testuser";
    private const string DefaultPassword = "somepassword";
    
    public TestContainerBuilder() : base(new TestContainerConfiguration())
    {
        DockerResourceConfiguration = new();
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    public TestContainerBuilder(TestContainerConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    public string UserName => DefaultUsername;
    public string Password => DefaultPassword;

    protected override TestContainerBuilder Init() =>
        base.Init()
            .WithImage($"postgres:{DefaultVersion}")
            .WithPortBinding(5432, true)
            .WithName("fotowebb-test-db")
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(5432));
 
    public override TestContainer Build()
    {
        Validate();
        return new TestContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    protected override TestContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TestContainerConfiguration(resourceConfiguration));
    }
    protected override TestContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        // return this;
        return Merge(DockerResourceConfiguration, new TestContainerConfiguration(resourceConfiguration));
    }

    protected override TestContainerBuilder Merge(TestContainerConfiguration oldValue, TestContainerConfiguration newValue)
    {
        return new TestContainerBuilder(new TestContainerConfiguration(oldValue, newValue));
    }

    protected override TestContainerConfiguration DockerResourceConfiguration { get; } 
    
    
    public TestContainerBuilder WithUsername(string username)
    {
        var result = Merge(DockerResourceConfiguration, new TestContainerConfiguration(username: username));
        return result.WithEnvironment("POSTGRES_USER", DefaultUsername);
    }

    public TestContainerBuilder WithPassword(string password)
    {
        var result = Merge(DockerResourceConfiguration, new TestContainerConfiguration(password: password));
        return result.WithEnvironment("POSTGRES_PASSWORD", DefaultPassword);
    }
}

