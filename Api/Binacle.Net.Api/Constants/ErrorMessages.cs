namespace Binacle.Net.Api.Constants;

public static class ErrorMessages
{
	public const string IsRequired = "Is required.";
	public const string MalformedRequestBody = "Malformed request body.";
	public const string GreaterThanZero = "Must be greater than 0";

	public static readonly string LessThanUShortMaxValue = string.Format("Must be less than {0}", ushort.MaxValue);
}
