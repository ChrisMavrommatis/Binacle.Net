using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared decode-reject vectors: raw blobs the decoder must reject. The test only asserts that
// deserialize throws — the exception type and message differ per language, so the shared contract is
// just "rejected", documented by each case's Reason.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class DecodeInvalidTests
{
	[Theory]
	[MemberData(nameof(DecodeInvalidProvider.Names), MemberType = typeof(DecodeInvalidProvider))]
	public void Deserialize_Rejects_Invalid_Blob(string name)
	{
		var scenario = DecodeInvalidProvider.Get(name);

		// Deliberately broad. Each blob rejects at a specific stage (short-of-header, reserved version, reserved
		// bit, reserved width code, missing item count, truncated body, trailing bytes — see each Reason), but
		// the exception type differs per stage and per language. The shared cross-language contract is just
		// "rejected", so this asserts only that it throws; the C#-specific stages are pinned by type in
		// SerializationBehaviorTests.
		Should.Throw<Exception>(() =>
			ViPaqSerializer.Deserialize<Binacle.Geometry.Dimensions<long>, Binacle.Geometry.Item<long>, long>(scenario.Blob));
	}
}
