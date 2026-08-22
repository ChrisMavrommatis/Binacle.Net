using Binacle.Net.UIModule.Services;

namespace Binacle.Net.UIModule.UnitTests;

// The applet list drives the index cards and every demo page's own heading. It is hand-written, so what is
// checked here is that it stays usable as a lookup key rather than that it says anything in particular.
[Trait("Behavioral Tests", "Ensures the applet list stays usable as a page lookup")]
public class AppletsServiceTests
{
	[Fact]
	public void Every_Applet_Page_Is_Unique()
	{
		var service = new AppletsService();

		var pages = service.Applets.Select(x => x.Page).ToList();

		pages.Distinct().Count().ShouldBe(pages.Count);
	}

	[Fact]
	public void Every_Applet_Carries_The_Copy_Both_The_Card_And_The_Page_Render()
	{
		var service = new AppletsService();

		var applets = service.Applets;

		applets.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Title));
		applets.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Icon));
		applets.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.ShortDescription));
		applets.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Description));
	}
}
