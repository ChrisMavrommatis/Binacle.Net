using System.IO.Compression;

namespace Binacle.ViPaq.Compression;

// Gzip (RFC 1952) - the same DEFLATE stream as `DeflateCodec`, wrapped in ~18 bytes of magic, mtime, OS byte
// and a CRC trailer. That wrapper is redundant here, but it is recognisable in a hex dump and it is what gets
// raced against raw DEFLATE. `GZipStream` pairs with `CompressionStream('gzip')` in a browser.
//
// A .NET quirk: on this data `CompressionLevel.Optimal` produced a smaller blob than `SmallestSize`. Level
// never reaches the wire, so it can change freely.
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
