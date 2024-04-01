using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.ServiceModule.Configuration;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	internal const string UsersApiName = "users";

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

		
		var info = new OpenApiInfo()
		{
			Title = "Binacle Users API",
			Version = "v1",
			Description = "Binacle Users API for User Management. <br> This section is designed only for when Binacle is used as public service. <br> User Management is done only by a user of Admin Group.",
			License = new OpenApiLicense() { Name = "View on Github", Url = new Uri("https://github.com/ChrisMavrommatis/Binacle.Net") },
		};

		options.SwaggerDoc(UsersApiName, info);
	}

	internal static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
	{
		options.SwaggerEndpoint($"/swagger/{UsersApiName}/swagger.json", $"Binacle Users API");
	}
}
