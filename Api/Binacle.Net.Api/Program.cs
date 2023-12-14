using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Options.Models;
using Binacle.Net.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Binacle.Net.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            // Slim builder
            builder.WebHost.UseKestrelHttpsConfiguration();

            // Slim builder
            builder.WebHost.UseQuic(); //HTTP 3 support

            builder.Configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/App_Data");

            builder.Services.AddValidatorsFromAssemblyContaining<IApiMarker>(ServiceLifetime.Singleton);

            builder.Configuration.AddJsonFile(BinPresetOptions.Path, optional: false, reloadOnChange: true);
            builder.Services
               .AddOptions<BinPresetOptions>()
               .Bind(builder.Configuration.GetSection(BinPresetOptions.SectionName))
               .ValidateFluently()
               .ValidateOnStart();

            builder.Services.AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
                setup.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader()
                    );
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddSingleton<ILockerService, LockerService>();

            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                //options.OperationFilter<ExamplesOperationFilter>();
                options.SchemaFilter<PresetQueryRequestSchemaFilter>();
                //options.SchemaFilter<QueryRequestSchemaFilter>();
            });
            // Swashbuckle.AspNetCore.Filters
            // builder.Services.AddSwaggerExamples();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseQueryStrings = true;
                options.LowercaseUrls = true;
            });

            var app = builder.Build();

            // Slim builder
            app.UseHttpsRedirection();

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