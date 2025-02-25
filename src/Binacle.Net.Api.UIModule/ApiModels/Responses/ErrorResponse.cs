namespace Binacle.Net.Api.UIModule.ApiModels.Responses;

internal class ErrorResponse
{
	public ResponseResultType Result { get; set; }

	public List<Error>? Data { get; set; }
	
	public string? Message { get; set; }
}
