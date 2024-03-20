using Microsoft.AspNetCore.Builder;

namespace ChrisMavrommatis.Api.MinimalEndpoints;

public interface IEndpointDefinition
{
	void DefineEndpoints(WebApplication app);
}
