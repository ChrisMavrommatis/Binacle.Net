using Binacle.Net.Api.ServiceModule.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Api.ServiceModule.Domain;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDomainLayerServices(this IServiceCollection services)
	{
		services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);
		services.AddScoped<IUserManagerService, UserManagerService>();

		return services;
	}
}
