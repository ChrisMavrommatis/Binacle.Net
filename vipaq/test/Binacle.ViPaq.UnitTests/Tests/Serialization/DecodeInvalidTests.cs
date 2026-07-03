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

		// Deliberately broad: these blobs reject for four different reasons (ArgumentException,
		// ArgumentOutOfRangeException, EndOfStreamException, InvalidDataException), so the shared contract
		// is just "throws" — matching the TypeScript side, which only asserts it rejects.
		Should.Throw<Exception>(() =>
			ViPaqSerializer.Deserialize<Bin<long>, Item<long>, long>(scenario.Blob));
	}
}
