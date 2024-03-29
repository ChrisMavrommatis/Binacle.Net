namespace Binacle.Net.Api.ServiceModule.ApiModels.Responses;

internal class DescriptiveErrorResponse
{
	public string Message { get; set; }
	public string[]? Errors { get; set; }
	internal static DescriptiveErrorResponse Create(string message, string[]? errors = null)
	{
		return new DescriptiveErrorResponse
		{
			Message = message,
			Errors = errors
		};
	}
}
