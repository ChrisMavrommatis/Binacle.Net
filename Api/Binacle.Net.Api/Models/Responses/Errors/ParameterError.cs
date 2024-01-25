namespace Binacle.Net.Api.Models.Responses.Errors;

public class ParameterError : IApiError
{
    public string Parameter { get; set; }
    public string Mesasage { get; set; }
}
