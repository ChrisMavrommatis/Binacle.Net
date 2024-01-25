namespace Binacle.Net.Api.Models.Responses;

public class HealthCheckResponse : ResponseBase
{
    public HealthCheckResponse(string version, string section)
    {
        Version = version;
        Section = section;
    }

    public string Section { get; set; }
    public string Version { get; set; }
}
