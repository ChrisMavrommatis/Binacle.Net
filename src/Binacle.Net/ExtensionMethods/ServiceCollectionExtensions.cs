using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.Net.Services;

namespace Binacle.Net.ExtensionMethods;

internal static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBinacleServices(
		this IServiceCollection services
		)
	{
		services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
		services.AddSingleton<IBinProcessorFactory, BinProcessorFactory>();
		services.AddSingleton<IAlgorithmProcessorFactory, AlgorithmProcessorFactory>();
		services.AddSingleton<IBinacleService, BinacleService>();

		return services;
	}
	
}
