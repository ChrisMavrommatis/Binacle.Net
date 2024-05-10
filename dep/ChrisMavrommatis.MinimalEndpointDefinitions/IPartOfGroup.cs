using Microsoft.AspNetCore.Routing;

namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public interface IPartOfGroup
{
	void DefineEndpoint(RouteGroupBuilder group);
}
