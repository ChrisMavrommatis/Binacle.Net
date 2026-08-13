namespace Binacle.ViPaq;

// Base64 is not part of the wire (PROTOCOL.md describes bytes, not text). It is how a blob rides inside JSON.
// These two make that trip one call, not two:
//
//   var token = ViPaqSerializer.Serialize<Bin, Item, int>(bin, items).ToBase64();
//   var (bin, items) = ViPaqSerializer.Deserialize<Bin, Item, int>(token.FromBase64());
//
// Nothing about a blob is checked here.
public static class ViPaqBase64Extensions
{
	public static string ToBase64(this byte[] blob)
	{
		ArgumentNullException.ThrowIfNull(blob);

		return Convert.ToBase64String(blob);
	}

	// Throws FormatException on text that is not base64, the same as Convert.FromBase64String - catch that
	// alongside ViPaqFormatException.
	public static byte[] FromBase64(this string base64)
	{
		ArgumentNullException.ThrowIfNull(base64);

		return Convert.FromBase64String(base64);
	}
}
