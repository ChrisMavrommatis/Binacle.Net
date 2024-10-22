namespace Binacle.Net.Api.v2.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ExceptionError : IApiError
{
	public required string ExceptionType { get; set; }
	public required string Message { get; set; }
	public string? StackTrace { get; set; }
}
