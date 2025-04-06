using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.Domain.Configuration.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Domain;

public static class DomainSetup
{
	public static T AddDomainLayerServices<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

		builder.AddValidatableJsonConfigurationOptions<UserOptions>();

		builder.Services.AddScoped<IUserManagerService, UserManagerService>();

		return builder;

	}
}
