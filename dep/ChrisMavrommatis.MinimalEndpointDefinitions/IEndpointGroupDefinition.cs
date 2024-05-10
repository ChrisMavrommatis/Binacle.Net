using Microsoft.AspNetCore.Routing;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public interface IEndpointGroupDefinition 
{
	RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints);
}
