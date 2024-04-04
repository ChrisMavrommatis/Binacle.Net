using ChrisMavrommatis.SwaggerExamples.Services;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChrisMavrommatis.SwaggerExamples;

public static class SwaggerGenOptionsExtensions
{
	public static void UseSwaggerExamples(this SwaggerGenOptions options)
	{
		options.OperationFilter<SwaggerExamplesOperationFilter>();
	}
}
