using Binacle.Net.ServiceModule.Application.Accounts.Services;
using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Application.Subscriptions.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;
using YetAnotherMediator;

namespace Binacle.Net.ServiceModule.Application.Authentication.UseCases;

public record AuthenticationRequest(string Username, string Password)
	: IRequest<FluxUnion<Token, Unauthorized, UnexpectedError>>;

internal class AuthenticationRequestHandler: IRequestHandler<AuthenticationRequest, FluxUnion<Token, Unauthorized, UnexpectedError>>
{
	private readonly IAccountRepository accountRepository;
	private readonly ISubscriptionRepository subscriptionRepository;
	private readonly ITokenService tokenService;
	private readonly IPasswordHasher passwordHasher;

	public AuthenticationRequestHandler(
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		ITokenService tokenService,
		IPasswordHasher passwordHasher
		
		)
	{
		this.accountRepository = accountRepository;
		this.subscriptionRepository = subscriptionRepository;
		this.tokenService = tokenService;
		this.passwordHasher = passwordHasher;
	}
	
	public async ValueTask<FluxUnion<Token, Unauthorized, UnexpectedError>> HandleAsync(AuthenticationRequest request, CancellationToken cancellationToken)
	{
		var accountResult = await this.accountRepository.GetByUsernameAsync(request.Username);
		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return TypedResult.Unauthorized;
		}

		if (!account.IsActive() || !account.HasPassword())
		{
			return TypedResult.Unauthorized;
		}

		if (this.passwordHasher.PasswordMatches(request.Password, account.PasswordHash!))
		{
			return TypedResult.Unauthorized;
		}

		Subscription? subscription = null;
		if (account.HasSubscription())
		{
			var subscriptionResult = await this.subscriptionRepository.GetByAccountIdAsync(account.Id);
			subscriptionResult.TryGetValue<Subscription>(out var foundSubscription);
			if(foundSubscription is not null && foundSubscription.IsActive())
			{
				subscription = foundSubscription;
			}
		}

		var tokenResult = this.tokenService.GenerateToken(account, subscription);
		if (tokenResult.TryGetValue<Token>(out var token) && token is not null)
		{
			return token;
		}

		return TypedResult.UnexpectedError;
	}
}
