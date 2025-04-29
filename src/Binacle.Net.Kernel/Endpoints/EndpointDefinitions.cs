using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Kernel.Endpoints;

public interface IGroupedEndpoint : IEndpointDefinition
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

public interface IEndpoint : IEndpointDefinition
{
	void DefineEndpoint(IEndpointRouteBuilder endpoints);
}

public interface IEndpointDefinition;
