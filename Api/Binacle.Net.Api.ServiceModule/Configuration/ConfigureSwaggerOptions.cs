using Binacle.Net.Api.ServiceModule.Services;
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
			Title = "Binacle.Net Users API",
			Version = "v1",
			Description = __description__,
			// gpl 3 license
			License = new OpenApiLicense
			{
				Name = "GNU General Public License v3.0",
				Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
			}
		};

		options.SwaggerDoc(UsersApiName, info);

		options.OperationFilter<RateLimitingResponsesFilter>();
	}

	internal static void ConfigureSwaggerUI(SwaggerUIOptions options, WebApplication app)
	{
		options.SwaggerEndpoint($"/swagger/{UsersApiName}/swagger.json", $"Binacle.Net Users API");
	}

	private const string __description__ = """
		**Binacle.Net Users API for User Management**
		
		This section is designed only for when Binacle is used as public service. <br>
		User Management is done only by a user of Admin Group.
		
		[View on Github](https://github.com/ChrisMavrommatis/Binacle.Net)

		[Get Postman collection](https://www.postman.com/chrismavrommatis/workspace/binacle-net)

		""";
}
