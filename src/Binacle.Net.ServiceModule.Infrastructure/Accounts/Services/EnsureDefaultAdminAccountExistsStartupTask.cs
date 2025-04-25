using Binacle.Net.ServiceModule.Application.Accounts.UseCases;
using Binacle.Net.ServiceModule.Application.Common.Configuration;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class EnsureDefaultAdminAccountExistsStartupTask : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	private const string _defaultAdminAccount = "admin@binacle.net:B1n4cl3Adm!n"; 

	public EnsureDefaultAdminAccountExistsStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using (var scope = this.serviceProvider.CreateScope())
		{
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

			// get configuration
			var options = scope.ServiceProvider.GetRequiredService<IOptions<ServiceModuleOptions>>();
			var configuredAdminCredentials = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

			var request = new CreateAccountCommand(
				configuredAdminCredentials.Username.ToLowerInvariant(),
				configuredAdminCredentials.Password,
				configuredAdminCredentials.Username.ToLowerInvariant(),
				AccountRole.Admin
			);

			var result = await mediator.ExecuteAsync(request, cancellationToken);

			var success = result.Match(
				account => true,
				conflict => true,
				error => false
			);

			if (!success)
			{
				throw new ApplicationException($"{nameof(EnsureDefaultAdminAccountExistsStartupTask)} failed");
			}
		}

	}


}
