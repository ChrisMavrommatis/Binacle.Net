using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Services;
using ChrisMavrommatis.StartupTasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.ServiceModule.Infrastructure.Services;

internal class EnsureAdminUserExistsStartupTask : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureAdminUserExistsStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using (var scope = this.serviceProvider.CreateScope())
		{
			var defaultsOptions = scope.ServiceProvider.GetRequiredService<IOptions<DefaultsOptions>>();
			var defaultsOptionsValidator = scope.ServiceProvider.GetRequiredService<IValidator<DefaultsOptions>>();

			await defaultsOptionsValidator.ValidateAndThrowAsync(defaultsOptions.Value);

			var service = scope.ServiceProvider.GetRequiredService<IUserManagerService>();

			var configuredAdminUser = defaultsOptions.Value.GetParsedAdminUser();

			var request = new Domain.Users.Models.CreateUserRequest(configuredAdminUser.Email, configuredAdminUser.Password, UserGroups.Admins);

			var result = await service.CreateAsync(request, cancellationToken);

			var success = result.Unwrap(
				user => true,
				conflict => true,
				error => false
				);

			
			if (!success)
			{
				throw new System.ApplicationException($"{nameof(EnsureAdminUserExistsStartupTask)} failed");
			}
		}
		
	}
}
