namespace Binacle.Net.Api.Kernel.OpenApi.Models;

internal sealed class ResponseDescriptionMetadata
{
	internal ResponseDescriptionMetadata(int statusCode, string description)
	{
		this.StatusCode = statusCode;
		this.Description = description;
	}

	public int StatusCode { get; }
	public string Description { get; }
}
