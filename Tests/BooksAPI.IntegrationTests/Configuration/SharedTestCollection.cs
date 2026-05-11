namespace BooksAPI.IntegrationTests.Configuration;

[CollectionDefinition("ApiTests")]
public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>;
