namespace Binacle.PackingVisualizationProtocol.Models;

public struct Header
{
	// [1 Byte]
	public EncodingInfo EncodingInfo { get; set; }
	// [2 Bytes]
	public ushort NumberOfItems { get; init; }
}
