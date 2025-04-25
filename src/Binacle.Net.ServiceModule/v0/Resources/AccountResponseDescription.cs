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
	
	public static string For409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account with the same username already exists.")
		.AppendLine()
		.ToString();
}

internal static class GetAccountResponseDescription
{
	public static string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account exists")
		.AppendLine()
		.ToString();
}

internal static class ListAccountResponseDescription
{
	public static string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When accounts exist")
		.AppendLine()
		.ToString();
	
	public static string For404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When no accounts exist")
		.AppendLine()
		.ToString();
}

internal static class CreateAccountResponseDescription
{
	public static string For201Created = new StringBuilder("**Created**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have successfully created an account.")
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
