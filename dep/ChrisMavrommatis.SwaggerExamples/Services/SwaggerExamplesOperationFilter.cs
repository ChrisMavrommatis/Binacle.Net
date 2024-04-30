using ChrisMavrommatis.SwaggerExamples.Abstractions;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChrisMavrommatis.SwaggerExamples.Services;


internal class SwaggerExamplesOperationFilter : IOperationFilter
{
	private readonly ExamplesWriter writer;

	public SwaggerExamplesOperationFilter(ExamplesWriter writer)
	{
		this.writer = writer;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		this.ApplyRequestExamples(operation, context);
		this.ApplyResponseExamples(operation, context);
	}

	private void ApplyRequestExamples(OpenApiOperation operation, OperationFilterContext context)
	{
		var actionAttributes = context.GetExampleAttributes<SwaggerRequestExampleAttribute>();

		foreach (var attr in actionAttributes)
		{
			var exampleProvider = Activator.CreateInstance(attr.ExamplesProviderType);
			if (exampleProvider is ISingleSwaggerExamplesProvider singleExampleProvider)
			{
				writer.WriteSingleRequest(operation, singleExampleProvider);
			}

			if (exampleProvider is IMultipleSwaggerExamplesProvider multipleExamplesProvider)
			{
				writer.WriteMultipleRequests(operation, multipleExamplesProvider);
			}
		}
	}

	private void ApplyResponseExamples(OpenApiOperation operation, OperationFilterContext context)
	{
		var actionAttributes = context.GetExampleAttributes<SwaggerResponseExampleAttribute>();

		foreach (var attr in actionAttributes)
		{
			var exampleProvider = Activator.CreateInstance(attr.ExamplesProviderType);
			if (exampleProvider is ISingleSwaggerExamplesProvider singleExampleProvider)
			{
				writer.WriteSingleResponse(operation, attr.StatusCode, singleExampleProvider);
			}

			if (exampleProvider is IMultipleSwaggerExamplesProvider multipleExamplesProvider)
			{
				writer.WriteMultipleResponses(operation, attr.StatusCode, multipleExamplesProvider);
			}
		}
	}

}
