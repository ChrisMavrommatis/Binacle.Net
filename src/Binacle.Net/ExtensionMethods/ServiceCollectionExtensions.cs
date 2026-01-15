using Binacle.Net.Services;
using Binacle.Lib;
using Binacle.Lib.Abstractions;

namespace Binacle.Net.ExtensionMethods;

internal static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBinacleServices(
		this IServiceCollection services
		)
	{
		services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
		services.AddKeyedSingleton<IBinProcessor, LoopBinProcessor>("loop");
		services.AddKeyedSingleton<IBinProcessor, ParallelBinProcessor>("parallel");
		services.AddSingleton<IBinacleService, BinacleService>();
		return services;
	}
	
}
