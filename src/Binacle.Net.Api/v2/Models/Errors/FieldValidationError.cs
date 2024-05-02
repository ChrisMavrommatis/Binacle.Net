namespace Binacle.Net.Api.v2.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class FieldValidationError : IApiError
{
	public string Field { get; set; }
	public string Error { get; set; }
}
