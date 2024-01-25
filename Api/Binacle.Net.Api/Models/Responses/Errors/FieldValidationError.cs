namespace Binacle.Net.Api.Models.Responses.Errors;

public class FieldValidationError : IApiError
{
    public string Field { get; set; }
    public string Error { get; set; }
}
