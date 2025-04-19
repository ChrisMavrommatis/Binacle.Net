using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Binacle.Net.ServiceModule.Application.Authentication.Configuration;
using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using FluxResults;
using FluxResults.TypedResults;
using FluxResults.Unions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;

internal class TokenService : ITokenService
{
	private readonly IOptions<JwtAuthOptions> options;
	private readonly TimeProvider timeProvider;

	public TokenService(
		IOptions<JwtAuthOptions> options,
		TimeProvider timeProvider
	)
	{
		this.options = options;
		this.timeProvider = timeProvider;
	}

	public FluxUnion<Token, UnexpectedError> GenerateToken(Account account, Subscription? subscription)
	{
		try
		{
			var claims = new List<Claim>()
			{
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
				new(JwtRegisteredClaimNames.Name, account.Username),
				new(JwtRegisteredClaimNames.Email, account.Email),
				new(ClaimTypes.Role, account.Role.ToString()),
				new(ApplicationClaimTypes.SecurityStamp, account.SecurityStamp.ToString())
			};

			if (subscription is not null)
			{
				claims.Add(new (ApplicationClaimTypes.Subscription, subscription.Id.ToString()));
				claims.Add(new (ApplicationClaimTypes.SubscriptionType, subscription.Type.ToString()));
			}

			var now = this.timeProvider.GetLocalNow();

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.options.Value.TokenSecret!));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = now.DateTime.AddSeconds(this.options.Value.ExpirationInSeconds),
				Issuer = this.options.Value.Issuer,
				Audience = this.options.Value.Audience,
				IssuedAt = now.DateTime,
				NotBefore = now.DateTime,
				SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.CreateToken(tokenDescriptor);

			var token = tokenHandler.WriteToken(securityToken);
			return new Token(token, JwtBearerDefaults.AuthenticationScheme, this.options.Value.ExpirationInSeconds);
		}
		catch (Exception ex)
		{
			return TypedResult.UnexpectedError;
		}
	}
}
