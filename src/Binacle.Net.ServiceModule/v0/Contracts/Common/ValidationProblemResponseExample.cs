using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

#pragma warning disable CS8618
internal abstract class ValidationProblemResponseExample : ISingleOpenApiExamplesProvider<ProblemDetails>
{
	public abstract Dictionary<string, string[]> GetErrors();
	
	public IOpenApiExample<ProblemDetails> GetExample()
	{
		return OpenApiExample.Create(
			"validationProblem",
			"Validation Problem",
			new HttpValidationProblemDetails(GetErrors())
		);
	}
}
