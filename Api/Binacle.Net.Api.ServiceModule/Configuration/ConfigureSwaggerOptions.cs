using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.ServiceModule.Configuration;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	public void Configure(string? name, SwaggerGenOptions options)
	{
		this.ConfigureSwaggerGenOptions(options);

	}

	public void Configure(SwaggerGenOptions options)
	{
		this.ConfigureSwaggerGenOptions(options);
	}

	private void ConfigureSwaggerGenOptions(SwaggerGenOptions options)
	{
		options.EnableAnnotations();
		var jwtSecurityScheme = new OpenApiSecurityScheme
		{
			BearerFormat = "JWT",
			Name = "JWT Authentication",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.Http,
			Description = "Please provide a valid token",
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			Reference = new OpenApiReference
			{
				Id = JwtBearerDefaults.AuthenticationScheme,
				Type = ReferenceType.SecurityScheme
			}
		};

		options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

		options.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			[jwtSecurityScheme] = new List<string>()
		});
	}
}
