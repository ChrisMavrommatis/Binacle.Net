using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared encode-reject vectors: (bin, items) inputs the serializer must reject end-to-end. The shared
// contract is just "rejected" (the exception type/message differ per language), documented by each
// scenario's Reason. C# can assert tighter than that: every reject path here — the dimension/coordinate
// pickers and the item-count guard — throws ArgumentOutOfRangeException, so we pin that exact type.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class EncodeInvalidTests
{
	[Theory]
	[MemberData(nameof(EncodeInvalidProvider.Names), MemberType = typeof(EncodeInvalidProvider))]
	public void Serialize_Rejects_Invalid_Input(string name)
	{
		var scenario = EncodeInvalidProvider.Get(name);

		Should.Throw<ArgumentOutOfRangeException>(() =>
			ViPaqSerializer.Serialize<Bin<long>, Item<long>, long>(scenario.Bin, scenario.Items));
	}
}
