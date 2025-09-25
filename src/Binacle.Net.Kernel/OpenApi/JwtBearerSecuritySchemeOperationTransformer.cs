using Binacle.Net.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Kernel.OpenApi;

internal class JwtBearerSecuritySchemeOperationTransformer : IOpenApiOperationTransformer
{
	private readonly IAuthenticationSchemeProvider? authenticationSchemeProvider;

	public JwtBearerSecuritySchemeOperationTransformer(
		IOptionalDependency<IAuthenticationSchemeProvider> authenticationSchemeProvider
	)
	{
		this.authenticationSchemeProvider = authenticationSchemeProvider.Value;
	}

	public async Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		if (authenticationSchemeProvider is null)
		{
			return;
		}

		var hasAuthorizeAttribute =
			context.Description.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>().Any();
		if (!hasAuthorizeAttribute)
		{
			return;
		}

		var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
		if (!authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
		{
			return;
		}

		operation.Security.Add(new OpenApiSecurityRequirement
		{
			[
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Id = "Bearer",
						Type = ReferenceType.SecurityScheme
					}
				}
			] = Array.Empty<string>()
		});
	}
}
