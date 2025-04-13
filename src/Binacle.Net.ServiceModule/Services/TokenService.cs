using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Binacle.Net.ServiceModule.Configuration.Models;
using Binacle.Net.ServiceModule.Models;

namespace Binacle.Net.ServiceModule.Services;

internal interface ITokenService
{
	StatelessTokenGenerationResult GenerateStatelessToken(StatelessTokenGenerationRequest request);
}

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

	public StatelessTokenGenerationResult GenerateStatelessToken(StatelessTokenGenerationRequest request)
	{
		if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.UserGroup))
		{
			return new StatelessTokenGenerationResult(false);
		}

		var claims = new List<Claim>()
		{
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new(JwtRegisteredClaimNames.Sub, request.Email),
			new(JwtRegisteredClaimNames.Email, request.Email),
			new(JwtApplicationClaimNames.Groups, request.UserGroup),
			new(ClaimTypes.Role, request.UserGroup)
		};

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
		return new StatelessTokenGenerationResult(true, token, JwtBearerDefaults.AuthenticationScheme, this.options.Value.ExpirationInSeconds);
	}
}
