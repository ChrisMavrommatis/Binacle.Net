namespace Binacle.Net.Api.v1.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ParameterError : IApiError
{
	public required string Parameter { get; set; }
	public required string Message { get; set; }
}
