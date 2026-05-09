namespace AuthAPI.IntegrationTests.Configuration;

[CollectionDefinition("AuthApiIntegrationTests")]
public class SharedTestCollection : ICollectionFixture<AuthApiWebApplicationFactory>
{
}

