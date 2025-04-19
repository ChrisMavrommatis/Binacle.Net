using Binacle.Net.ServiceModule.Application.Authentication.Models;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenResponse
{
	public required string TokenType { get; set; }
	public required string AccessToken { get; set; }
	public int ExpiresIn { get; set; }
	public string? RefreshToken { get; set; }

	public static TokenResponse Create(Token token)
	{
		return new TokenResponse
		{
			TokenType = token.TokenType,
			AccessToken = token.TokenValue,
			ExpiresIn = token.ExpiresIn,
		};
	}
}
