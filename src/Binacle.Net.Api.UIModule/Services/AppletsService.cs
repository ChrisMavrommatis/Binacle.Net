namespace Binacle.Net.Api.UIModule.Services;

internal class AppletsService
{
	public List<Models.Applet> Applets { get; private set; }
	
	public AppletsService()
	{
		this.Applets = new List<Models.Applet>
		{
			new Models.Applet
			{
				Title = "Packing Demo",
				Icon = "deployed_code",
				ShortDescription = "Allows you to visualize how Binacle.Net packs them into a container using it's algorithms.",
				Description = "An interactive tool that lets test different packing algorithms with your own items and bins, and explore detailed visualizations of how items are efficiently arranged within a container.",
				Ref = "PackingDemo"
			},
			new Models.Applet
			{
				Title = "Protocol Decoder",
				Icon = "deployed_code_update",
				ShortDescription = "Decode, analyze, and visualize packing layouts with the ViPaq Protocol",
				Description = "The Protocol Decoder enables you to decode ViPaq-encoded packing data, providing clear, interactive visualizations of container layouts for easy analysis and validation.",
				Ref = "ProtocolDecoder"
			},
		};
		
	}
}
