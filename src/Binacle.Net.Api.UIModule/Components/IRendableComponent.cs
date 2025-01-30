using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components;

internal interface IRendableContentComponent
{
	RenderFragment Render();
}
