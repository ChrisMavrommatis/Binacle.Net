using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Application;

public static class Setup
{
	public static T AddApplication<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		return builder;
	}
	
}
