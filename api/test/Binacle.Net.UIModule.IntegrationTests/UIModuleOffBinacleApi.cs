namespace Binacle.Net.UIModule.IntegrationTests;

// The same image with the demo switched off. Every page route has to be gone, and nothing may start
// answering with a web page.
public sealed class UIModuleOffBinacleApi : UIModuleApi
{
	public UIModuleOffBinacleApi() : base(uiModuleEnabled: false)
	{
	}
}
