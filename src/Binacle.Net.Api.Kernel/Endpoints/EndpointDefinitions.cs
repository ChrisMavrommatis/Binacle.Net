using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api.Kernel.Endpoints;

public interface IGroupedEndpoint
{
	void DefineEndpoint(RouteGroupBuilder group);
}

public interface IEndpointGroup
{
	RouteGroupBuilder DefineEndpointGroup(IEndpointRouteBuilder endpoints);
}
public interface IGroupedEndpoint<TGroup> : IGroupedEndpoint
	where TGroup : class, IEndpointGroup
{
}

public interface IEndpoint
{
	void DefineEndpoint(IEndpointRouteBuilder endpoints);
}

