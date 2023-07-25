using Foto.Tests.TestContainer;

namespace Foto.Tests;

[CollectionDefinition("Integration tests collection")]
public class IntegrationTestsCollection :  ICollectionFixture<TestContainerLifeTime>
{

}