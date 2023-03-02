using Binacle.Api.BoxNow.Configuration;
using Binacle.Api.Components.Application;
using Binacle.Api.Components.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Api.BoxNow
{
    public class Startup : IBuilderSetup
    {
        public int SequenceOrder => 20;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile(BoxNowOptions.Path, optional: false, reloadOnChange: true);
            builder.Services
               .AddOptions<BoxNowOptions>()
               .Bind(builder.Configuration.GetSection(BoxNowOptions.SectionName))
               .ValidateFluently()
               .ValidateOnStart();
        }
    }
}
