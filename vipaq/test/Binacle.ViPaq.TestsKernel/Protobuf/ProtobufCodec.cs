using System.IO.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Google.Protobuf;

namespace Binacle.ViPaq.TestsKernel.Protobuf;

// Turns a sample into protobuf bytes — the baseline ViPaq is measured against. It offers both a raw and a
// gzip'd form so the harness can obey the fairness rule: compare against the raw form when ViPaq did not
// compress, and against both forms when it did. The gzip settings match ViPaq's own (System.IO gzip,
// CompressionLevel.Optimal), so the only thing that differs is the format, not the codec.
public static class ProtobufCodec
{
	public static byte[] Encode(PackingSample sample) 
		=> ToMessage(sample)
			.ToByteArray();

	public static string ToBase64(byte[] bytes) 
		=> Convert.ToBase64String(bytes);

	public static byte[] EncodeGzip(PackingSample sample)
	{
		var raw = Encode(sample);

		using var output = new MemoryStream();
		using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
		{
			gzip.Write(raw, 0, raw.Length);
		}

		return output.ToArray();
	}

	private static PackedResult ToMessage(PackingSample sample)
	{
		var message = new PackedResult
		{
			Count = (uint)sample.Items.Length,
			Bin = new Vec3
			{
				Length = sample.Bin.Length,
				Width = sample.Bin.Width,
				Height = sample.Bin.Height
			}
		};

		foreach (var item in sample.Items)
		{
			message.Items.Add(new PlacedItem
			{
				Length = item.Length,
				Width = item.Width,
				Height = item.Height,
				X = item.X,
				Y = item.Y,
				Z = item.Z
			});
		}

		return message;
	}
}
