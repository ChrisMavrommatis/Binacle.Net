namespace Binacle.Net.Api.Kernel.OpenApi.Models;

internal sealed class ResponseDescriptionMetadata
{
	internal ResponseDescriptionMetadata(int statusCode, string description)
	{
		this.StatusCode = statusCode;
		this.Description = description;
	}

	public int StatusCode { get; set; }
	public string Description { get; set; }
}
