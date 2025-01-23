using Binacle.Net.Api.UIModule.Components.Models;

namespace Binacle.Net.Api.UIModule.Components.Services;

internal class ThemeService
{
	public Models.Theme DefaultTheme { get; private set; } = Models.Theme.Light;
}
