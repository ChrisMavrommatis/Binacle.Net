namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class ErrorMessage
{
	public static string RequiredEnumValues<TEnum>(string propertyName)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();

		return
			$"'{nameof(propertyName)}' is required and must be one of the following values: {string.Join(", ", values)}";
	}
}
