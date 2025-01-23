using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Models;

internal interface IRendableContentComponent
{
	RenderFragment Render();
}
