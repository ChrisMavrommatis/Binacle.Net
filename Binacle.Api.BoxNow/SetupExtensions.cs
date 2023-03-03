using Binacle.Api.BoxNow.Configuration;
using Binacle.Api.Components.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Api.BoxNow
{
    public static class SetupExtensions
    {
        public static void UseBoxNow(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<IBoxNowMarker>(ServiceLifetime.Singleton);

            builder.Configuration.AddJsonFile(BoxNowOptions.Path, optional: false, reloadOnChange: true);
            builder.Services
               .AddOptions<BoxNowOptions>()
               .Bind(builder.Configuration.GetSection(BoxNowOptions.SectionName))
               .ValidateFluently()
               .ValidateOnStart();
        }
    }
}
