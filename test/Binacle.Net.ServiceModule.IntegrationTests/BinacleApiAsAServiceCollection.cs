
namespace Binacle.Net.ServiceModule.IntegrationTests;

[CollectionDefinition(Name)]
public class BinacleApiAsAServiceCollection : ICollectionFixture<BinacleApiAsAServiceFactory>
{
	public const string Name = "Binacle Api As A Service Collection";
	// This class has no code, and is never created. Its purpose is simply
	// to be the place to apply [CollectionDefinition] and all the
	// ICollectionFixture<> interfaces.
}
