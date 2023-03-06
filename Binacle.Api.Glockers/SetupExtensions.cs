using Binacle.Api.Glockers.Models;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Binacle.Api.Components.Extensions;

namespace Binacle.Api.Glockers
{
    public static class SetupExtensions
    {
        public static void UseGlockers(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<IGlockersMarker>(ServiceLifetime.Singleton);

            builder.Configuration.AddJsonFile(GlockersOptions.Path, optional: false, reloadOnChange: true);
            builder.Services
               .AddOptions<GlockersOptions>()
               .Bind(builder.Configuration.GetSection(GlockersOptions.SectionName))
               .ValidateFluently()
               .ValidateOnStart();
        }
    }
}
