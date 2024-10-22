using System.Text.Json.Serialization;

namespace Binacle.Net.Api.UIModule.ApiModels;

public class Error
{
	[JsonPropertyName("$type")]
	public string? TypeName { get; set; }
	public string? Parameter { get; set; }
	public string? Message { get; set; }

	public string? Field { get; set; }

	[JsonPropertyName("error")]
	public string? FieldError { get; set; }
}
