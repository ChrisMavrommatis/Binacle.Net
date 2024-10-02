using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Api.ServiceModule.Domain;

public static class DomainSetup
{
	public static T AddDomainLayerServices<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(ServiceLifetime.Singleton, includeInternalTypes: true);

		builder.Configuration
			.AddJsonFile(UserOptions.FilePath, optional: false, reloadOnChange: false)
			.AddEnvironmentVariables();

		builder.Services
			.AddOptions<UserOptions>()
			.Bind(builder.Configuration.GetSection(UserOptions.SectionName));

		builder.Services.AddScoped<IUserManagerService, UserManagerService>();

		return builder;

	}
}
