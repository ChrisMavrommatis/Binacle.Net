using System.IO.Compression;

namespace Binacle.ViPaq.Compression;

// Gzip (RFC 1952) — the same DEFLATE stream as `DeflateCodec`, wrapped in ~18 bytes of magic, mtime, OS byte
// and a CRC trailer.
//
// The wrapper is redundant here (the header already says the body is compressed, and the body knows its own
// length), but it is what the old format used and it is the more recognisable thing to find in a hex dump.
// Kept so it can be raced against raw DEFLATE on real packs. `GZipStream` pairs with
// `CompressionStream('gzip')` in a browser.
//
// A .NET quirk worth knowing before reading a benchmark: on this data `CompressionLevel.Optimal` produced a
// *smaller* blob than `SmallestSize`. Level never reaches the wire, so it can change freely.
internal sealed class GzipCodec : ICompressionCodec
{
	public byte[] Compress(ReadOnlySpan<byte> body)
	{
		using var output = new MemoryStream();
		using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
		{
			gzip.Write(body);
		}

		return output.ToArray();
	}

	public byte[] Decompress(ReadOnlySpan<byte> compressed)
	{
		using var input = new MemoryStream(compressed.ToArray());
		using var gzip = new GZipStream(input, CompressionMode.Decompress);
		using var output = new MemoryStream();

		try
		{
			gzip.CopyTo(output);
		}
		catch (InvalidDataException exception)
		{
			throw new ViPaqFormatException("The compressed body is not a valid gzip stream", exception);
		}

		return output.ToArray();
	}
}
