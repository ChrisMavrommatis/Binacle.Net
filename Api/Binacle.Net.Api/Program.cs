using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Api.Options.Models;
using Binacle.Net.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Binacle.Net.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/App_Data");

            builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(ServiceLifetime.Singleton);

            builder.Configuration.AddJsonFile(BinPresetOptions.Path, optional: false, reloadOnChange: true);
            builder.Services
               .AddOptions<BinPresetOptions>()
               .Bind(builder.Configuration.GetSection(BinPresetOptions.SectionName))
               .ValidateFluently()
               .ValidateOnStart();

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

            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}