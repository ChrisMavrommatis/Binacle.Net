namespace Binacle.Net.v1.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomQueryRequest
{
	public List<v1.Models.Bin>? Bins { get; set; }
	public List<v1.Models.Box>? Items { get; set; }
}
