namespace ChrisMavrommatis.MinimalEndpointDefinitions;

public interface IEndpointDefinition<TGroup> : IPartOfGroup
	where TGroup: class, IEndpointGroupDefinition
{
}
