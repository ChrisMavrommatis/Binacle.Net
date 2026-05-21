using Microsoft.AspNetCore.Components;

namespace Binacle.Net.UIModule.Components;

internal interface IRendableContentComponent
{
	RenderFragment Render();
}
