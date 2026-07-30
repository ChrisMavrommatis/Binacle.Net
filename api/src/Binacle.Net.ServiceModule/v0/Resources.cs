namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class ResponseDescription
{
    public static string For400BadRequest = "The request is malformed or invalid";
    public static string For401Unauthorized = "Authentication failed due to invalid or missing credentials";
    public static string For403Forbidden = "Access is denied due to insufficient permissions";
    public static string For422UnprocessableContent = "The request is well-formed but contains validation errors";
    public static string For500InternalServerError = "An unexpected error occurred on the server";
}

internal static class SubscriptionResponseDescription
{
	public static string For404NotFound = "The account does not have a subscription or the account doesn't exist";
	public static string For409Conflict = "The account already has a subscription";
}

internal static class AccountResponseDescription
{
	public static string For404NotFound = "The account with the specified ID does not exist";
	public static string For409Conflict = "An account with the same username already exists";
}

internal static class ErrorMessage
{
	public static string RequiredEnumValues<TEnum>(string propertyName)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();

		return
			$"'{propertyName}' is required and must be one of the following values: {string.Join(", ", values)}";
	}

	public const string IdMustBeGuid = "The provided value for 'Id' must be a valid Guid";
}
