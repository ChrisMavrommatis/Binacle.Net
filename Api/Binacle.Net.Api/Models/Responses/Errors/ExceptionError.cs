namespace Binacle.Net.Api.Models.Responses.Errors;

public class ExceptionError : IApiError
{
	public string ExceptionType { get; set; }
	public string Message { get; set; }
	public string StackTrace { get; set; }
}
