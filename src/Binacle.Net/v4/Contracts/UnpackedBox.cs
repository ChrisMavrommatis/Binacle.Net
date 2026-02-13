using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class UnpackedBox : IWithID
{
	public required string ID { get; set; }
	public required int Quantity { get; set; }
}
