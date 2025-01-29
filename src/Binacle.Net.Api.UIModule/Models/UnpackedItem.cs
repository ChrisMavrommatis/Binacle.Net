using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.UIModule.Models;

internal class UnpackedItem :IWithID, IWithQuantity
{
	// Json Deserialize
	public UnpackedItem()
	{
		this.ID = string.Empty;
	}
	
	public UnpackedItem(string id, int quantity)
	{
		this.ID = id;
		this.Quantity = quantity;
	}
	public required string ID { get; set; }
	public int Quantity { get; set; }
}
