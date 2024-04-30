using Microsoft.AspNetCore.Mvc;

namespace ChrisMavrommatis.Endpoints;

public static class MvcOptionsExtensions
{
	/// <summary>
	/// Allows to use "[namespace]" as part of a route.
	/// </summary>
	public static MvcOptions UseNamespaceRouteToken(this MvcOptions options)
	{
		options.Conventions.Add(new Services.CustomRouteToken(
			"namespace",
			c => c.ControllerType.Namespace?.Split('.').Last()
		));

		return options;
	}
}
