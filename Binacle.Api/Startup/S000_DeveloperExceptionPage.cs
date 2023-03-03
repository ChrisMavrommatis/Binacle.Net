using Binacle.Api.Components.Application;

namespace Binacle.Api.Startup
{
    public class DeveloperExceptionPage : IApplicationStartup
    {
        public int SequenceOrder => 0;

        public void Execute(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
