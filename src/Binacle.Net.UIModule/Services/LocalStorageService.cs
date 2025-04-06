using Microsoft.JSInterop;

namespace Binacle.Net.UIModule.Services;

internal class LocalStorageService
{
	private readonly IJSRuntime js;

	public LocalStorageService(IJSRuntime js)
	{
		this.js = js;
	}
	
	public async ValueTask<T?> GetItemAsync<T>(string key)
	{
		var json = await this.js.InvokeAsync<string>("localStorage.getItem", key);
		if (string.IsNullOrEmpty(json))
		{
			return default;
		}
		return System.Text.Json.JsonSerializer.Deserialize<T>(json);
	}
	
	public async ValueTask SetItemAsync<T>(string key, T value)
	{
		var json = System.Text.Json.JsonSerializer.Serialize(value);
		await this.js.InvokeVoidAsync("localStorage.setItem", key, json);
	}

}
