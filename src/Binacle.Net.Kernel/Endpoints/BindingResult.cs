using Microsoft.AspNetCore.Http;

namespace Binacle.Net.Kernel.Endpoints;


public record struct BindingResult<T>(
	T? Value,
	Exception? Exception
)
{
	public static async ValueTask<BindingResult<T>> BindAsync(HttpContext httpContext)
	{
		try
		{
			var item = await httpContext.Request.ReadFromJsonAsync<T>();
			return new(item, null);
		}
		catch (Exception ex)
		{
			return new(default, ex);
		}
	}
}
