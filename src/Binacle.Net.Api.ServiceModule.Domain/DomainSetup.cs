using Binacle.Net.Api.Kernel;
using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.ServiceModule.Domain;

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
