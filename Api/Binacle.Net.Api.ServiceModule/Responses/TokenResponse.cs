namespace Binacle.Net.Api.ServiceModule.Responses;

internal class TokenResponse
{
	public string TokenType { get; set; }
	public string AccessToken { get; set; }
	public int ExpiresIn { get; set; }
	public string? RefreshToken { get; set; }

	public static TokenResponse Create(string tokenType, string accessToken, int expiresIn, string? refreshToken = null)
	{
		return new TokenResponse
		{
			TokenType = tokenType,
			AccessToken = accessToken,
			ExpiresIn = expiresIn,
			RefreshToken = refreshToken
		};
	}

}
