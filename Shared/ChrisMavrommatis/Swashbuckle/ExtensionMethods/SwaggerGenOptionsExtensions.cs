using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
	public static SwaggerGenOptions IncludeXmlCommentsFromAssembly(this SwaggerGenOptions options, Assembly assembly)
	{
		var xmlFilename = $"{assembly.GetName().Name}.xml";
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
		return options;
	}

	public static SwaggerGenOptions AddPolymorphicTypeMappings(this SwaggerGenOptions options, Dictionary<Type, Type[]> polymorphicTypeMappings)
	{
		options.SelectSubTypesUsing(baseType =>
		{
			if (polymorphicTypeMappings.ContainsKey(baseType))
			{
				return polymorphicTypeMappings[baseType];
			}

			return Enumerable.Empty<Type>();
		});

		return options;
	}

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

