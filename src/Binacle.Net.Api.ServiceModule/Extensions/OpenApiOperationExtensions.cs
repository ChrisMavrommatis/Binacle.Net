using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api.ServiceModule.Extensions;

internal static class OpenApiOperationExtensions
{
	internal static void SetResponseDescription(this OpenApiOperation operation, string statusCode, string description)
	{
		if(operation.Responses.TryGetValue(statusCode, out var response))
		{
			response.Description = description;
		}
	}

	internal static void SetResponseDescription(this OpenApiOperation operation, int statusCode, string description)
		=> SetResponseDescription(operation, statusCode.ToString(), description);
	
}
