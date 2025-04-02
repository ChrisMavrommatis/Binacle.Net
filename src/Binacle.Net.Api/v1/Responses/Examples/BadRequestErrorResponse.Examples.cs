// using ChrisMavrommatis.SwaggerExamples;
// using ChrisMavrommatis.SwaggerExamples.Abstractions;
//
// namespace Binacle.Net.Api.v1.Responses.Examples;
//
// internal class BadRequestErrorResponseExamples : MultipleSwaggerExamplesProvider<ErrorResponse>
// {
// 	public override IEnumerable<ISwaggerExample<ErrorResponse>> GetExamples()
// 	{
// 		yield return SwaggerExample.Create("Validation Error", "Validation Error", "Example response with validation errors",
// 			ErrorResponse.Create(Constants.Errors.Categories.ValidationError)
// 			.AddFieldValidationError("Items[0].Height", Constants.Errors.Messages.GreaterThanZero)
// 			);
//
// 		yield return SwaggerExample.Create("Request Error", "Request Error", "Example response when the request is malformed",
// 			ErrorResponse.Create(Constants.Errors.Categories.RequestError)
// 				.AddParameterError("request", Constants.Errors.Messages.MalformedRequestBody)
// 			);
// 	}
// }
