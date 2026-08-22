namespace Binacle.Net.UIModule.Services;

internal class AppletsService
{
	public IReadOnlyList<Models.Applet> Applets { get; }
	
	public AppletsService()
	{
		this.Applets = new List<Models.Applet>
		{
			new Models.Applet
			{
				Title = "Packing Demo",
				Icon = "deployed_code",
				ShortDescription = "Put in your own bins and items, pick an algorithm, and watch Binacle.Net pack them.",
				Description = "An interactive tool that lets you test different packing algorithms with your own bins and items, and see how each one arranges them inside the bin.",
				Page = "/Packing"
			},
			new Models.Applet
			{
				Title = "ViPaq Decoder",
				Icon = "deployed_code_update",
				ShortDescription = "Decode, analyze, and visualize packing layouts with the ViPaq Protocol",
				Description = "The ViPaq Decoder enables you to decode ViPaq-encoded packing data, providing clear, interactive visualizations of container layouts for easy analysis and validation.",
				Page = "/Vipaq"
			},
			new Models.Applet
			{
				Title = "This Instance",
				Icon = "monitoring",
				ShortDescription = "What this container is running, what is switched on, and the presets it loaded.",
				Description = "What this container is running, what is switched on, and the presets it loaded. Everything here is read from the instance you are on, so it answers whether your own configuration arrived the way you meant it to.",
				Page = "/Instance"
			},
		};
		
	}
}
