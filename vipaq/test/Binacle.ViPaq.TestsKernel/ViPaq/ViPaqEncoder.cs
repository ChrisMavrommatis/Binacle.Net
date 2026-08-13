using System.Runtime.CompilerServices;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.ViPaq;

// The harness's door into ViPaq for the codec race. It drives the blind `ProtocolEncoder` directly, not the
// public `ViPaqSerializer.Serialize` — that wrapper decides the mode for you (row-major, uncompressed), and the
// race's whole job is to force every mode. So it borrows one thing from the wrapper, the width choice
// (`Header.Create`, reachable here only through InternalsVisibleTo), then flips `Compressed` and `Layout` for
// the mode and hands the encoder the mode's codec. It never re-derives a width — that spec lives in
// `Header.Create` alone.
//
// Every scenario is ushort, so that is the integer type both calls name; ViPaq itself decides 8- vs 16-bit per
// section from the values. Base64 is the real stored form and the headline number, so a report reads size off
// `ToBase64`, while BenchmarkDotNet measures the raw bytes `Encode` hands back.
public class ViPaqEncoder
{
	private readonly ICompressionCodec compressionCodec;

	public ViPaqEncoder(ICompressionCodec compressionCodec)
	{
		this.compressionCodec = compressionCodec;
	}
	
	public byte[] Encode(Scenario scenario, EncoderInfo encoderInfo)
	{
		var vipaqHeader = ViPaqHeader.Create(scenario, encoderInfo);
		var encoder = new ProtocolEncoder(this.compressionCodec);
		return encoder.Encode<Dimensions<ushort>, Item<ushort>, ushort>(vipaqHeader.Header, scenario.Bin, scenario.Items);
	}
	
	// The header on the wire already carries the layout and the compressed bit, so decode only needs the mode to
	// know which codec inflates the body. The two header bytes are stripped first — `ProtocolEncoder.Decode` wants
	// everything after them, the body only.
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode(byte[] token, ViPaqHeader header)
	{
		var encoder = new ProtocolEncoder(this.compressionCodec);
		return encoder.Decode<Dimensions<ushort>, Item<ushort>, ushort>(header.Header, token[Header.ByteCount..]);
	}
}
