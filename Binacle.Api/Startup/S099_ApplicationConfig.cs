using Binacle.Api.Components.Application;

namespace Binacle.Api.Startup
{
    public class ApplicationConfig : IApplicationStartup
    {
        public int SequenceOrder => 99;

        public void Execute(WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
