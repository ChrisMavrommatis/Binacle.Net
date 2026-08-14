using System.Runtime.CompilerServices;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.ViPaq;

// The harness's door into ViPaq for the codec race. It drives the blind `ProtocolEncoder` directly, because
// `ViPaqSerializer.Serialize` fixes the mode at row-major and uncompressed and the race has to force every
// mode. It borrows the width choice from `Header.Create` (reachable through InternalsVisibleTo), flips
// `Compressed` and `Layout`, and hands the encoder the mode's codec. It never re-derives a width.
//
// Every scenario is ushort. Base64 is the real stored form, so a report reads size off `ToBase64`, while
// BenchmarkDotNet measures the raw bytes `Encode` hands back.
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
	
	// The header on the wire already carries the layout and the compressed bit, so decode only needs the mode
	// to pick the codec. The two header bytes are stripped first: `ProtocolEncoder.Decode` wants the body only.
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode(byte[] token, ViPaqHeader header)
	{
		var encoder = new ProtocolEncoder(this.compressionCodec);
		return encoder.Decode<Dimensions<ushort>, Item<ushort>, ushort>(header.Header, token[Header.ByteCount..]);
	}
}
