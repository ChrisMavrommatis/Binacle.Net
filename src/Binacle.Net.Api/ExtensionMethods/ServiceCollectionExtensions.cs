using Binacle.Net.Api.Services;
using Binacle.Net.Lib;
using Binacle.Net.Lib.Abstractions;

namespace Binacle.Net.Api.ExtensionMethods;

internal static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBinacleServices(
		this IServiceCollection services
		)
	{
		services.AddSingleton<IAlgorithmFactory, AlgorithmFactory>();
		services.AddKeyedSingleton<IBinProcessor, LoopBinProcessor>("loop");
		services.AddKeyedSingleton<IBinProcessor, ParallelBinProcessor>("parallel");
		services.AddSingleton<ILegacyBinsService, LegacyBinsService>();
		services.AddSingleton<IBinacleService, BinacleService>();
		return services;
	}
	
}
