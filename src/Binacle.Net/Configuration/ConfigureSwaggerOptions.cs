// using System.Text;
// using Asp.Versioning.ApiExplorer;
// using ChrisMavrommatis.Endpoints;
// using ChrisMavrommatis.SwaggerExamples;
// using ChrisMavrommatis.Swashbuckle;
// using Microsoft.Extensions.Options;
// using Microsoft.OpenApi.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
// using Swashbuckle.AspNetCore.SwaggerUI;
//
// namespace Binacle.Net.Configuration;
//
// internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
// {
// 	private readonly IApiVersionDescriptionProvider _provider;
// 	private static IApiVersion[] apiVersions = [
// 		new v1.ApiVersion(),
// 		new v2.ApiVersion(),
// 		new v3.ApiVersion()
// 	];
// 	
// 	public ConfigureSwaggerOptions(
// 		IApiVersionDescriptionProvider provider
// 		)
// 	{
// 		_provider = provider;
// 	}
//
// 	public void Configure(string? name, SwaggerGenOptions options)
// 	{
// 		this.ConfigureSwaggerGenOptions(options);
// 	}
//
// 	public void Configure(SwaggerGenOptions options)
// 	{
// 		this.ConfigureSwaggerGenOptions(options);
// 	}
//
// 	private void ConfigureSwaggerGenOptions(SwaggerGenOptions options)
// 	{
// 		options.IncludeXmlCommentsFromAssemblyContaining<IApiMarker>();
// 		options.UseSwaggerExamples();
// 		options.UseOneOfForPolymorphism();
//
// 		options.TagActionsByEndpointNamespaceOrDefault();
// 		options.DescribeAllParametersInCamelCase();
//
// 		foreach(var apiVersion in apiVersions)
// 		{
// 			apiVersion.ConfigureSwaggerOptions(options, _provider);
// 		}
// 	}
//
// 	public static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
// 	{
// 		options.RoutePrefix = "swagger";
//
// 		var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// 		foreach(var apiVersion in apiVersions)
// 		{
// 			apiVersion.ConfigureSwaggerUI(options, apiVersionDescriptionProvider);
// 		}
// 	}
// }
