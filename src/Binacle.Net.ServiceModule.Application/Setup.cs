using Binacle.Net.ServiceModule.Application.Accounts.UseCases;
using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Binacle.Net.ServiceModule.Application.Authentication.UseCases;
using Binacle.Net.ServiceModule.Application.Common.Configuration;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using FluentValidation;
using FluxResults.TypedResults;
using FluxResults.Unions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application;

public static class Setup
{
	public static T AddApplication<T>(this T builder)
		where T : IHostApplicationBuilder
	{
		builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>(ServiceLifetime.Singleton,
			includeInternalTypes: true);

		// builder.Services.AddRequestHandler<AuthenticationRequest, FluxUnion<Token, Unauthorized, UnexpectedError>, AuthenticationRequestHandler>();
		// builder.Services.AddQueryHandler<GetAccountQuery, FluxUnion<Account, NotFound>, GetAccountQueryHandler>();
		// builder.Services.AddCommandHandler<CreateAccountCommand, FluxUnion<Account, Conflict, UnexpectedError>, CreateAccountCommandHandler>();
		// builder.Services.AddCommandHandler<UpdateAccountCommand, FluxUnion<Success, NotFound, Conflict, UnexpectedError>, UpdateAccountCommandHandler>();
		// builder.Services.AddCommandHandler<DeleteAccountCommand, FluxUnion<Success, NotFound, UnexpectedError>, DeleteAccountCommandHandler>();
		builder.Services.AddRequestHandler<AuthenticationRequestHandler>();
		builder.Services.AddQueryHandler<GetAccountQueryHandler>();
		builder.Services.AddQueryHandler<ListAccountsQueryHandler>();
		builder.Services.AddCommandHandler<CreateAccountCommandHandler>();
		builder.Services.AddCommandHandler<UpdateAccountCommandHandler>();
		builder.Services.AddCommandHandler<DeleteAccountCommandHandler>();

		var defaultAdminCredentials = Environment.GetEnvironmentVariable("BINACLE_ADMIN_CREDENTIALS");
		if (!string.IsNullOrWhiteSpace(defaultAdminCredentials))
		{
			builder.Services.Configure<ServiceModuleOptions>(options =>
			{
				options.DefaultAdminAccount = defaultAdminCredentials;
			});
		}
		

		return builder;
	}
}
