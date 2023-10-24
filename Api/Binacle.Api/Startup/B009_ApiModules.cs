using Binacle.Api.Components.Application;
using Binacle.Api.Glockers;

namespace Binacle.Api.Startup
{
    public class ApiModules : IBuilderSetup
    {
        public int SequenceOrder => 2;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.UseGlockers();
        }
    }
}
