using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared decode-reject vectors: raw blobs the decoder must reject. The exception type and message differ per
// language, so the shared contract is just "rejected".
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class DecodeInvalidTests
{
	[Theory]
	[MemberData(nameof(DecodeInvalidProvider.Names), MemberType = typeof(DecodeInvalidProvider))]
	public void Deserialize_Rejects_Invalid_Blob(string name)
	{
		var scenario = DecodeInvalidProvider.Get(name);

		// Deliberately broad: the exception type differs per rejection stage and per language, so this asserts
		// only that it throws. The C#-specific stages are pinned by type in SerializationBehaviorTests.
		Should.Throw<Exception>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<long>, Binacle.Geometry.Item<long>, long>(scenario.Blob));
	}
}
