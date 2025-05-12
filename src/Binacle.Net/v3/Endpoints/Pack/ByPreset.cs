using Binacle.Net.Kernel.Endpoints;

namespace Binacle.Net.v3.Endpoints.Pack;

internal class ByPreset : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-preset/{preset}", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Preset")
			.WithDescription("Pack items using a preconfigured bin preset")
			;
	}

	internal async Task<IResult> HandleAsync(
		string preset
	)
	{
		throw new NotImplementedException();
	}
}
