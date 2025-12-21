using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.Kernel.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;

internal class EnsureDefaultAdminAccountExistsStartupTask : IStartupTask
{
	private readonly IServiceProvider serviceProvider;

	public EnsureDefaultAdminAccountExistsStartupTask(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using var scope = this.serviceProvider.CreateScope();

		var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
		var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
		
		// get configuration
		var options = scope.ServiceProvider.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var configuredAdminCredentials =
			ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		var getResult = await accountRepository.GetByUsernameAsync(
			configuredAdminCredentials.Username,
			cancellationToken
		);

		if (getResult.Is<Account>())
		{
			return;
		}
		
		var utcNow = timeProvider.GetUtcNow();
		var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
		
		var newAccount = new Account(
			configuredAdminCredentials.Username,
			AccountRole.Admin,
			configuredAdminCredentials.Email.ToLowerInvariant(),
			AccountStatus.Active,
			utcNow,
			Guid.CreateVersion7()
		);
		
		var password = passwordService.Create(configuredAdminCredentials.Password);
		newAccount.ChangePassword(password);
		var createResult = await accountRepository.CreateAsync(newAccount, cancellationToken);

		var success = createResult.Match(
			account => true,
			conflict => true
		);

		if (!success)
		{
			throw new ApplicationException($"{nameof(EnsureDefaultAdminAccountExistsStartupTask)} failed");
		}
	}
}
