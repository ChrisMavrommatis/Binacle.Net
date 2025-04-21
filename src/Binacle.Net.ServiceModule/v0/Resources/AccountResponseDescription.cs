using System.Text;

namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class AccountResponseDescription
{
	public static string For404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account does not exist.")
		.AppendLine()
		.ToString();
}

internal static class CreateAccountResponseDescription
{
	public static string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have successfully created an account.")
		.AppendLine()
		.ToString();

	public static string For409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account with the same email already exists.")
		.AppendLine()
		.ToString();
}

internal static class PatchAccountResponseDescription
{
	public static string For204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the password was changed.")
		.AppendLine()
		.ToString();

	public static string For409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the password is the same as the old.")
		.AppendLine()
		.ToString();
}

internal static class DeleteAccountResponseDescription
{
	public static string For204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account was deleted.")
		.AppendLine()
		.ToString();
}

internal static class UpdateAccountResponseDescription
{
	public static string For204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account was updated.")
		.AppendLine()
		.ToString();
}
