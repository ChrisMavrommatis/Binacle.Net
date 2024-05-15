using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

[CollectionDefinition(Name)]
public class BinacleApiCollection : ICollectionFixture<BinacleApiFactory>
{
	public const string Name = "Binacle Api Collection";
	// This class has no code, and is never created. Its purpose is simply
	// to be the place to apply [CollectionDefinition] and all the
	// ICollectionFixture<> interfaces.
}
