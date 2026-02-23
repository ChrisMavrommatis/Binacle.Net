namespace Binacle.Net.v3.ExtensionMethods;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddV3Services(
        this IServiceCollection services
    )
    {
        services.AddSingleton<Services.IBinacleService, Services.BinacleService>();
        return services;
    }
}
