using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcOptionsExtensions
{
	/// <summary>
	/// Allows to use "[namespace]" as part of a route.
	/// </summary>
	public static MvcOptions UseNamespaceRouteToken(this MvcOptions options)
	{
		options.Conventions.Add(new ChrisMavrommatis.Services.CustomRouteToken(
			"namespace",
			c => c.ControllerType.Namespace?.Split('.').Last()
		));

		return options;
	}
}
