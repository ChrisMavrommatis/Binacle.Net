using Binacle.Net.Kernel.OpenApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal abstract class ValidationProblemResponseExample : ISingleOpenApiExamplesProvider<ProblemDetails>
{
	public abstract Dictionary<string, string[]> GetErrors();
	
	public IOpenApiExample<ProblemDetails> GetExample()
	{
		return OpenApiValidationProblemExample.Create(GetErrors());
	}
}
