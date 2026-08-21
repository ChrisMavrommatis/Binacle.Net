using Binacle.Net.UIModule.Services;

namespace Binacle.Net.UIModule.Pages;

internal class VipaqModel : AppletPageModel
{
	public VipaqModel(AppletsService appletsService)
		: base(appletsService, "/Vipaq")
	{
	}

	public void OnGet()
	{
	}
}
