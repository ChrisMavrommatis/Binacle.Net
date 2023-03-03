using Binacle.Api.Components.Application;
using Binacle.Api.Components.Services;
using Binacle.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Binacle.Api.Startup
{
    public class ApiConfig : IBuilderSetup
    {
        public int SequenceOrder => 0;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/App_Data");

            builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
                setup.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader()
                    );
            });

            builder.Services.AddSingleton<ILockerService, LockerService>();
        }
    }
}
