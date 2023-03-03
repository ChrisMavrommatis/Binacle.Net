using Binacle.Api.BoxNow;
using Binacle.Api.Components.Application;

namespace Binacle.Api.Startup
{
    public class ApiModules : IBuilderSetup
    {
        public int SequenceOrder => 2;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.UseBoxNow();
        }
    }
}
