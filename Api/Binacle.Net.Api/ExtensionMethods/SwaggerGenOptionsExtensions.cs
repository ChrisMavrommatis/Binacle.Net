using Binacle.Net.Api.Models.Responses.Errors;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Binacle.Net.Api.ExtensionMethods;

public static class SwaggerGenOptionsExtensions
{
	public static SwaggerGenOptions IncludeXmlCommentsFromAssembly(this SwaggerGenOptions options)
	{
		var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
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
}
