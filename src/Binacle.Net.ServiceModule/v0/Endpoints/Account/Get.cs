using Binacle.Net.Kernel.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Account;

// TODO: Remove
internal class Get : IEndpoint
{
	public void DefineEndpoint(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/account/", HandleAsync)
			.WithSummary("Get an account");
	}

	internal async Task<IResult> HandleAsync()
	{
		throw new NotImplementedException();
	}

	
}
