using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api;

public static class OpenApiOperationExtensions
{
	public static void SetResponseDescription(this OpenApiOperation operation, string statusCode, string description)
	{
		if(operation.Responses.TryGetValue(statusCode, out var response))
		{
			response.Description = description;
		}
	}

	public static void SetResponseDescription(this OpenApiOperation operation, int statusCode, string description)
		=> SetResponseDescription(operation, statusCode.ToString(), description);
	
}
