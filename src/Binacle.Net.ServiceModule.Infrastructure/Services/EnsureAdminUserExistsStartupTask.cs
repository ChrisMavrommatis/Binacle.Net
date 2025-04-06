using Binacle.Net.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Users.Entities;
using Binacle.Net.ServiceModule.Domain.Users.Models;
using Binacle.Net.ServiceModule.Services;
using ChrisMavrommatis.StartupTasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Infrastructure.Services;

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
			var defaultsOptions = scope.ServiceProvider.GetRequiredService<IOptions<UserOptions>>();
			var defaultsOptionsValidator = scope.ServiceProvider.GetRequiredService<IValidator<UserOptions>>();

			await defaultsOptionsValidator.ValidateAndThrowAsync(defaultsOptions.Value);

			var service = scope.ServiceProvider.GetRequiredService<IUserManagerService>();

			var configuredAdminUser = defaultsOptions.Value.GetParsedDefaultAdminUser();

			var request = new CreateUserRequest(configuredAdminUser.Email, configuredAdminUser.Password, UserGroups.Admins);

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
