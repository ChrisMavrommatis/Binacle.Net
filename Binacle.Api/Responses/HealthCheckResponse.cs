using System.Runtime.Serialization;

namespace Binacle.Api.Responses
{
    public class HealthCheckResponse : ApiResponseBase
    {
        public HealthCheckResponse(string version, string section)
        {
            Version = version;
            Section = section;
        }

        public string Section { get; set; }
        public string Version { get; set; }
    }
}
