using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Components;

public partial class Breadcrumb : MaterializeComponentBase
{
	private List<BreadcrumbItem> items;

	[Parameter]
	public string WrapperClasses { get; set; }

	[Inject]
	protected NavigationManager NavigationManager { get; set; }

	protected override void OnParametersSet()
	{
		this.items = new List<BreadcrumbItem>();
		var currentUrl = NavigationManager.Uri;
		var myUrl = currentUrl.Replace(NavigationManager.BaseUri, "");
		var count = 0;
		this.items.Add(new BreadcrumbItem
		{
			Link = NavigationManager.BaseUri,
			IsActive = NavigationManager.Uri == NavigationManager.BaseUri,
			Order = count,
			Title = "Home",
			Icon = "home"
		});
		var path = myUrl.Split('/');

		foreach (var link in path)
		{
			if (link == "")
			{
				continue;
			}
			count++;
			var lastLink = this.items.Last();
			this.items.Add(new BreadcrumbItem
			{
				Link = $"{lastLink.Link}/{link}",
				IsActive = link == path.Last(),
				Order = count,
				Title = link
			});
		}
		base.OnParametersSet();
	}

	private class BreadcrumbItem
	{
		public int Order { get; set; }
		public required string Link { get; set; }
		public required string Title { get; set; }
		public string? Icon{ get; set; }
		public bool IsActive { get; set; }
	}
}


