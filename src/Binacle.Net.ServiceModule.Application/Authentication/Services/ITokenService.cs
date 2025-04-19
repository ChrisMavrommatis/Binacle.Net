using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults.TypedResults;
using FluxResults.Unions;

namespace Binacle.Net.ServiceModule.Application.Authentication.Services;


public interface ITokenService
{
	FluxUnion<Token, UnexpectedError> GenerateToken(Account account, Subscription? subscription);
}
