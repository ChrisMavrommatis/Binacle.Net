using Binacle.Net.UIModule.Services;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.Pages;

internal class PackingModel : AppletPageModel
{
	public PackingModel(AppletsService appletsService, IOptions<UIModuleOptions> options)
		: base(appletsService, "/Packing")
	{
		this.ApiBaseUrl = options.Value.ApiBaseUrl.TrimEnd('/');
	}

	public string ApiBaseUrl { get; }

	public void OnGet()
	{
	}
}
