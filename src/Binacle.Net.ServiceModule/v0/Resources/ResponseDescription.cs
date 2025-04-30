namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class ResponseDescription
{
    public static string For400BadRequest = "The request is malformed or invalid.";
    public static string For401Unauthorized = "Authentication failed due to invalid or missing credentials.";
    public static string For403Forbidden = "Access is denied due to insufficient permissions.";
    public static string For422UnprocessableEntity = "The request is well-formed but contains validation errors.";
    public static string For500InternalServerError = "An unexpected error occurred on the server.";
}
