using Binacle.Api.Configuration;
using Binacle.Api.Components.Application;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Binacle.Api.Components.Services;
using Binacle.Api.Services;

namespace Binacle.Api.Startup
{
    public class Configuration : IBuilderSetup
    {
        public int SequenceOrder => 0;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/App_Data");
        }
    }
    public class Validators : IBuilderSetup
    {
        public int SequenceOrder => 2;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            // This doesn't work unless its here
            builder.Services.AddValidatorsFromAssemblyContaining<BoxNow.Startup>();
        }
    }

    public class ApiConfig : IBuilderSetup
    {
        public int SequenceOrder => 3;

        public void Execute(WebApplicationBuilder builder)
        {
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
        }
    }

    public class Swagger : IBuilderSetup, IApplicationStartup
    {
        public int SequenceOrder => 10;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
        }

        public void Execute(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.RoutePrefix = string.Empty;
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            }
        }
    }

    public class AddLockerService : IBuilderSetup
    {
        public int SequenceOrder => 11;

        public void Execute(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ILockerService, LockerService>();
        }
    }

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

    public class ApiRunConfig : IApplicationStartup
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
