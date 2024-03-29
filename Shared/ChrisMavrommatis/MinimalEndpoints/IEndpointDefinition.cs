using Microsoft.AspNetCore.Builder;

namespace ChrisMavrommatis.MinimalEndpoints;

public interface IEndpointDefinition
{
	void DefineEndpoints(WebApplication app);
}
