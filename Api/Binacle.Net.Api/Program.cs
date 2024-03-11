﻿using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Binacle.Net.Api.Configuration;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Binacle.Net.Api;

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
		builder.Services.AddHealthChecks();
		builder.Configuration.AddJsonFile(BinPresetOptions.Path, optional: false, reloadOnChange: true);
		builder.Services
		   .AddOptions<BinPresetOptions>()
		   .Bind(builder.Configuration.GetSection(BinPresetOptions.SectionName))
		   .ValidateFluently()
		   .ValidateOnStart();

		builder.Services.AddControllers(options =>
		{
			options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
			options.UseNamespaceRouteToken();
		});

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
			options.ApiVersionReader = ApiVersionReader.Combine(
				new UrlSegmentApiVersionReader()
				);
		}).AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		builder.Services.AddSingleton<ILockerService, LockerService>();


		builder.Services.AddSwaggerGen();
		builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

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

		// Middleware are in order
		// Registered before Swagger because I don't want swagger to know about it
		app.MapHealthChecks("/_health");

		if (app.Environment.IsDevelopment())
		{
			var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.RoutePrefix = "swagger";
				foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
				{
					options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
				}
			});
		}


		app.UseHttpsRedirection();

		app.MapControllers();
		app.Run();
	}
}
