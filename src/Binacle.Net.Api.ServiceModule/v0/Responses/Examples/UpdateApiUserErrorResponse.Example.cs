using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.ServiceModule.v0.Responses.Examples;

internal class UpdateApiUserErrorResponseExample : MultipleSwaggerExamplesProvider<ErrorResponse>
{
	public override IEnumerable<ISwaggerExample<ErrorResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("Validation Error 1", "Validation Error 1", ErrorResponse.Create("Validation Error",
		[
			"'Email' is not a valid email address.",

		]));

		yield return SwaggerExample.Create("Validation Error 2", "Validation Error 2", ErrorResponse.Create("Validation Error",
		[
			"'Type' must not be empty when 'Status' is empty and only accepts the following values: 'User', 'Admin'",
			"'Status' must not be empty when 'Type' is empty and only accepts the following values: 'Active', 'Inactive'",
		]));
	}
}

