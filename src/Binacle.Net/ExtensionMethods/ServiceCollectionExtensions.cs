using Binacle.Lib;
using Binacle.Lib.Abstractions;
using Binacle.Net.v3.ExtensionMethods;
using Binacle.Net.v4.ExtensionMethods;

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

		services.AddV3Services();
		services.AddV4Services();
		return services;
	}
	
}
