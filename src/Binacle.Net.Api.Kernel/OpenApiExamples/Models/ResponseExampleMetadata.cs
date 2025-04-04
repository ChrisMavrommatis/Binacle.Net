namespace OpenApiExamples.Models;

internal class ResponseExampleMetadata
{
	public ResponseExampleMetadata(string statusCode, string contentType, Type providerType)
	{
		this.StatusCode = statusCode;
		this.ContentType = contentType;
		this.ProviderType = providerType;
	}

	public string StatusCode { get; }
	public string ContentType { get; }
	public Type ProviderType { get; }
}
