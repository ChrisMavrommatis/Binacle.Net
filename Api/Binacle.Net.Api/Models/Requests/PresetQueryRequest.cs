using Binacle.Net.Api.Examples;

namespace Binacle.Net.Api.Models.Requests;

public class PresetQueryRequest : IWithSwaggerExample<PresetQueryRequestExampleHolder>
{
	public List<Box> Items { get; set; }
}
