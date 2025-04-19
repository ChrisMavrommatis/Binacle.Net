namespace Binacle.Net.ServiceModule.Application.Authentication.Models;

public record Token(string TokenValue, string TokenType, int ExpiresIn);
