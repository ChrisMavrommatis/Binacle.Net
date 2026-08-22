using System.Collections.Generic;

namespace Binacle.Net.UIModule.Models;

// The other things this server can expose. FeatureOptions only records what is switched on, so the off half
// has to be listed somewhere - this is that list, and a new switch needs a row here to appear on the page.
//
// The service module is left out on purpose: it is not advertised, and the documentation site has no page for
// it. The demo UI is left out because you are looking at it.
internal record FeatureSwitch(string Feature, string Name, string Setting)
{
	public static IReadOnlyList<FeatureSwitch> All { get; } =
	[
		new("SwaggerUI", "Swagger UI", "SWAGGER_UI=True"),
		new("ScalarUI", "Scalar UI", "SCALAR_UI=True"),
		new("HealthChecks", "Health check", "HealthChecks__Enabled=True"),
		new("DebugEndpoint", "Debug endpoint", "DEBUG_ENDPOINT=True"),
	];
}
