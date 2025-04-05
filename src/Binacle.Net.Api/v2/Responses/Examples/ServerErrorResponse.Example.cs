﻿using Binacle.Net.Api.Constants;
using Binacle.Net.Api.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class ServerErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"Server Error",
			Response.ExceptionError(
				new InvalidOperationException("Example Exception"),
				ErrorCategory.ServerError
			)
		);
	}
}
