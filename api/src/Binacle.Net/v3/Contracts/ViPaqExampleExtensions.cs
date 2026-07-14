using Binacle.ViPaq;

namespace Binacle.Net.v3.Contracts;

// Fills an example result's ViPaqData from its own bin and placed items, so a documentation token can never drift
// from the geometry beside it (the old hardcoded tokens did, and were wrong). This mirrors PackResponse's runtime
// derivation exactly — same call, same codec — and runs only when OpenAPI examples are built, not on the request
// path.
internal static class ViPaqExampleExtensions
{
	public static BinPackResult WithViPaqData(this BinPackResult result)
	{
		if (result.PackedItems is { Count: > 0 })
		{
			result.ViPaqData = ViPaqSerializer
				.Serialize<Bin, PackedBox, int>(result.Bin, result.PackedItems)
				.ToBase64();
		}

		return result;
	}
}
