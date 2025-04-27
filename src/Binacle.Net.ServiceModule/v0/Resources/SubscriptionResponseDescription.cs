using System.Text;

namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class SubscriptionResponseDescription
{
	public static string For404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the subscription or the account does not exist.")
		.AppendLine()
		.ToString();
}

internal static class GetSubscriptionResponseDescription
{
	public static string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account has a subscription.")
		.AppendLine()
		.ToString();
}

internal static class CreateSubscriptionResponseDescription
{
	public static string For201Created = new StringBuilder("**Created**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have successfully created a subscription for the specified account.")
		.AppendLine()
		.ToString();

	public static string For409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account already has a subscription.")
		.AppendLine()
		.ToString();
}

internal static class UpdateSubscriptionResponseDescription
{
	public static string For204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the Subscription was updated.")
		.AppendLine()
		.ToString();
}

internal static class DeleteSubscriptionResponseDescription
{
	public static string For204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the Subscription was deleted.")
		.AppendLine()
		.ToString();
}


internal static class ListSubscriptionResponseDescription
{
	public static string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When subscriptions exist")
		.AppendLine()
		.ToString();
	
	public static string For404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When no subscriptions exist")
		.AppendLine()
		.ToString();
}
