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
				ShortDescription = "Create bins and items, then visualize how Binacle.Net packs them into a container using it's algorithms.",
				Description = "An interactive tool that lets you create bins and items, experiment with packing algorithms, and visualize how items are arranged within the container.",
				Ref = "PackingDemo"
			},
			new Models.Applet
			{
				Title = "Protocol Decoder",
				Icon = "deployed_code_update",
				ShortDescription = "Decode and visualize packing layouts using the Bin Packing Visualization Protocol",
				Description = "The Protocol Decoder allows you to paste a packing result encoded with the Bin Packing Visualization Protocol to decode and display the packing layout.",
				Ref = "ProtocolDecoder"
			},
		};
		
	}
}
