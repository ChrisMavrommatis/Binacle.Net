using ChrisMavrommatis.Endpoints.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChrisMavrommatis.Endpoints;

public static class SwaggerGenOptionsExtensions
{
	public static void TagActionsByEndpointNamespaceOrDefault(this SwaggerGenOptions swaggerGenOptions)
	{
		swaggerGenOptions.TagActionsBy(api =>
		{
			if (api.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
			{
				return api.ActionDescriptor.EndpointMetadata.OfType<TagsAttribute>().SelectMany(x => x.Tags).ToArray();
			}

			if (actionDescriptor.ControllerTypeInfo.GetBaseTypesAndThis().Any(t => t == typeof(ChrisMavrommatis.Endpoints.EndpointBase)))
			{
				return new[] { actionDescriptor.ControllerTypeInfo.Namespace?.Split('.').Last() };
			}

			return new[] { actionDescriptor.ControllerName };
		});
	}
}
