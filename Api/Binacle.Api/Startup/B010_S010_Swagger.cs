using Binacle.Api.Components.Application;
using Binacle.Api.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Binacle.Api.Startup
{
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
}
