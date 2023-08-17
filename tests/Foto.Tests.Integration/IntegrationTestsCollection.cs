using Foto.Tests.Integration.TestContainer;

namespace Foto.Tests.Integration;

[CollectionDefinition("Integration tests collection")]
public class IntegrationTestsCollection :  ICollectionFixture<TestContainerLifeTime>;