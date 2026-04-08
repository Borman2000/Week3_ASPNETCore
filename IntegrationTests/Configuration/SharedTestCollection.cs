namespace IntegrationTests.Configuration;

[CollectionDefinition("ApiTests")]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>;
