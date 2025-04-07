using Binacle.Net.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Kernel.OpenApi;

internal class JwtBearerSecuritySchemeDocumentTransformer : IOpenApiDocumentTransformer
{
	private readonly IAuthenticationSchemeProvider? authenticationSchemeProvider;

	public JwtBearerSecuritySchemeDocumentTransformer(
		IOptionalDependency<IAuthenticationSchemeProvider> authenticationSchemeProvider
	)
	{
		this.authenticationSchemeProvider = authenticationSchemeProvider.Value;
	}

	public async Task TransformAsync(
		OpenApiDocument document,
		OpenApiDocumentTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		if (authenticationSchemeProvider is null)
		{
			return;
		}

		var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
		if (!authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
		{
			return;
		}

		// Add the security scheme at the document level
		var requirements = new Dictionary<string, OpenApiSecurityScheme>
		{
			["Bearer"] = new OpenApiSecurityScheme
			{
				BearerFormat = "Json Web Token",
				Name = "JWT Authentication",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Description = "Please provide a valid JWT token",
				Scheme = "bearer", // "bearer" refers to the header name here
			}
		};
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes = requirements;
	}
}
