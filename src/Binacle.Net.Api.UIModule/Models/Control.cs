using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Models;

internal class Control
{
	private readonly Func<Task> onClick;

	public Control(string id, string icon, Func<Task> onClick)
	{
		this.Element = new ElementReference(id);
		this.Icon = icon;
		this.onClick = onClick;
	}

	public ElementReference Element { get; }
	public bool IsEnabled { get; set; }
	public string Icon { get; set; }

	public async Task ClickAsync()
	{
		await this.onClick.Invoke();
	}

}
