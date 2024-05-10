namespace ChrisMavrommatis.MinimalEndpointDefinitions.Models;

internal class EndpointGroupDefinition
{
	internal IEndpointGroupDefinition Definition { get; set; }

	internal IReadOnlyCollection<IPartOfGroup> EndpointDefinitions { get; set; }

}
