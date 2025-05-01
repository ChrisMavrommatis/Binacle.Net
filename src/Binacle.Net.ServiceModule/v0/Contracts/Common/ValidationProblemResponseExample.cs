using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal abstract class ValidationProblemResponseExample : ISingleOpenApiExamplesProvider<ProblemDetails>
{
	public abstract Dictionary<string, string[]> GetErrors();
	
	public IOpenApiExample<ProblemDetails> GetExample()
	{
		return OpenApiExample.Create(
			"validationProblem",
			"Validation Problem",
			new HttpValidationProblemDetails(GetErrors())
			{
				Type= "https://tools.ietf.org/html/rfc4918#section-11.2",
				Status = StatusCodes.Status422UnprocessableEntity
			}
		);
	}
}
