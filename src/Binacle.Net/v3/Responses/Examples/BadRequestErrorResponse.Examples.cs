// TODO: Example
// using Binacle.Net.Constants;
// using Binacle.Net.v3.Models;
// using OpenApiExamples;
// using OpenApiExamples.Abstractions;
//
// namespace Binacle.Net.v3.Responses.Examples;
//
// internal class BadRequestErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
// {
// 	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
// 	{
// 		yield return OpenApiExample.Create(
// 			"validationError",
// 			"Validation Error",
// 			"Example response with validation errors",
// 			Response.FieldValidationError("Items[0].Height", ErrorMessage.GreaterThanZero,
// 				ErrorCategory.ValidationError)
// 		);
//
// 		yield return OpenApiExample.Create(
// 			"requestError",
// 			"Request Error",
// 			"Example response when the request is malformed",
// 			Response.ParameterError("request", ErrorMessage.MalformedRequestBody, ErrorCategory.ValidationError)
// 		);
// 	}
// }
