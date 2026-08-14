using System.IO.Compression;

namespace Binacle.ViPaq.Compression;

// Raw DEFLATE (RFC 1951), no wrapper. Cheaper than gzip by ~18 bytes a blob, which pushes the point where
// compressing starts to pay closer in.
//
// `DeflateStream` pairs with `CompressionStream('deflate-raw')` in a browser and `zlib.createDeflateRaw` in
// Node. That is only proven by the cross-language round-trip tests - do not take it on trust.
internal sealed class DeflateCodec : ICompressionCodec
{
	public byte[] Compress(ReadOnlySpan<byte> body)
	{
		using var output = new MemoryStream();
		using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
		{
			deflate.Write(body);
		}

		return output.ToArray();
	}

	public byte[] Decompress(ReadOnlySpan<byte> compressed)
	{
		using var input = new MemoryStream(compressed.ToArray());
		using var deflate = new DeflateStream(input, CompressionMode.Decompress);
		using var output = new MemoryStream();

		try
		{
			deflate.CopyTo(output);
		}
		catch (InvalidDataException exception)
		{
			throw new ViPaqFormatException("The compressed body is not a valid DEFLATE stream", exception);
		}

		return output.ToArray();
	}
}
