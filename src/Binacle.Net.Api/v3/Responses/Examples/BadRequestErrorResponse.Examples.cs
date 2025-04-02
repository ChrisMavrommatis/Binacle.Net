// using Binacle.Net.Api.v3.Models;
// using ChrisMavrommatis.SwaggerExamples;
// using ChrisMavrommatis.SwaggerExamples.Abstractions;
//
// namespace Binacle.Net.Api.v3.Responses.Examples;
//
// internal class BadRequestErrorResponseExamples : MultipleSwaggerExamplesProvider<ErrorResponse>
// {
// 	public override IEnumerable<ISwaggerExample<ErrorResponse>> GetExamples()
// 	{
// 		yield return SwaggerExample.Create("Validation Error", "Validation Error", "Example response with validation errors",
// 			Response.FieldValidationError("Items[0].Height", Constants.Errors.Messages.GreaterThanZero, Constants.Errors.Categories.ValidationError)
// 			);
//
// 		yield return SwaggerExample.Create("Request Error", "Request Error", "Example response when the request is malformed",
// 			Response.ParameterError("request", Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.ValidationError)
// 			);
// 	}
// }
