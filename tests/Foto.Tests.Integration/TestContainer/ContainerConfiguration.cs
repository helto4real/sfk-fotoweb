using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace Foto.Tests.Integration.TestContainer;

public class TestContainerConfiguration : ContainerConfiguration
{
    public string Username { get; } = default!;
    public string Password { get; } = default!;
    
    public TestContainerConfiguration(
        string? username = null,
        string? password = null
        )
    {
        Username = username!;
        Password = password!;
    }
    
    public TestContainerConfiguration(TestContainerConfiguration oldValue, TestContainerConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }
    
    public TestContainerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        
    }

    public TestContainerConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
    }
}