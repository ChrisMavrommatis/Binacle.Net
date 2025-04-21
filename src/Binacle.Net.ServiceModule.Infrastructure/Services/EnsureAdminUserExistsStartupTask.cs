using Binacle.Net.ServiceModule.Application.Accounts.UseCases;
using ChrisMavrommatis.StartupTasks;
using Microsoft.Extensions.DependencyInjection;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Infrastructure.Services;

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

			var adminCredentials = Environment.GetEnvironmentVariable("BINACLE_ADMIN_CREDENTIALS");
			var configuredAdminCredentials = ParseAccountCredentials(adminCredentials ?? _defaultAdminAccount);

			var request = new CreateAccountCommand(
				configuredAdminCredentials.Email.ToLowerInvariant(),
				configuredAdminCredentials.Password,
				configuredAdminCredentials.Email.ToLowerInvariant()
			);

			var result = await mediator.ExecuteAsync(request, cancellationToken);

			var success = result.Match(
				user => true,
				conflict => true,
				error => false
			);

			if (!success)
			{
				throw new ApplicationException($"{nameof(EnsureDefaultAdminAccountExistsStartupTask)} failed");
			}
		}

	}

	private static ConfiguredAccountCredentials ParseAccountCredentials(string accountCredentials)
	{
		if (string.IsNullOrWhiteSpace(accountCredentials))
		{
			throw new ArgumentNullException(nameof(accountCredentials), "Account credentials cannot be null or empty");
		}

		var parts = accountCredentials.Split(":");
		return new ConfiguredAccountCredentials
		{
			Email = parts[0],
			Password = parts[1]
		};
	}

	private class ConfiguredAccountCredentials
	{
		public required string Email { get; init; }
		public required string Password { get; init; }
	}
}
