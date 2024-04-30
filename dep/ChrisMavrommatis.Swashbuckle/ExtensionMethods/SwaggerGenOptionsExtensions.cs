using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ChrisMavrommatis.Swashbuckle;

public static class SwaggerGenOptionsExtensions
{
	public static SwaggerGenOptions IncludeXmlCommentsFromAssemblyContaining<T>(this SwaggerGenOptions options)
		=> IncludeXmlCommentsFromAssembly(options, typeof(T).Assembly);

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
}

