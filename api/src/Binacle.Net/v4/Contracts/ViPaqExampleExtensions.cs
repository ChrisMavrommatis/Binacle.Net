using Binacle.ViPaq;

namespace Binacle.Net.v4.Contracts;

// Fills an example response's ViPaqData from its own bin and placed items, so a documentation token can never
// drift from the geometry beside it (the old hardcoded tokens did, and were wrong). This mirrors
// BinResponseBase's runtime derivation exactly — same call, same codec — and runs only when OpenAPI examples are
// built, not on the request path.
internal static class ViPaqExampleExtensions
{
	public static T WithViPaqData<T>(this T response)
		where T : BinResponseBase
	{
		if (response.PackedItems is { Count: > 0 })
		{
			response.ViPaqData = ViPaqSerializer
				.Serialize<Bin, PackedBox, int>(response.Bin, response.PackedItems)
				.ToBase64();
		}

		return response;
	}
}
