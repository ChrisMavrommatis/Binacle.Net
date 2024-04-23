using Microsoft.AspNetCore.Builder;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public interface IEndpointDefinition
{
	void DefineEndpoints(WebApplication app);
}
