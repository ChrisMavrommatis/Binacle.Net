using ChrisMavrommatis.SwaggerExamples.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ChrisMavrommatis.SwaggerExamples;

internal static class OperationFilterContextExtensions
{
	internal static IEnumerable<T> GetExampleAttributes<T>(this OperationFilterContext context)
		where T : SwaggerExamplesAttribute
	{
		return context.MethodInfo.GetCustomAttributes<T>();
	}

}
