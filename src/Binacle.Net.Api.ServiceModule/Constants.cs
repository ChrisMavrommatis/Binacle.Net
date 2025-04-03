using System.Text;

namespace Binacle.Net.Api.ServiceModule.Constants;


internal static class ResponseDescription
{
	public static string For400BadRequest = new StringBuilder("**Bad Request**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the request is invalid.")
		.AppendLine()
		.ToString();
	
	public static string For401Unauthorized = new StringBuilder("**Unauthorized**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When provided user token is invalid.")
		.AppendLine()
		.ToString();
	
	public static string For403Forbidden = new StringBuilder("**Forbidden**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When provided user token does not have permission.")
		.AppendLine()
		.ToString();
	
	public static string ForAuthToken200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have valid credentials.")
		.AppendLine()
		.ToString();
	public static string ForAuthToken401Unauthorized = new StringBuilder("**Unauthorized**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the credentials are invalid.")
		.AppendLine()
		.ToString();
	
	
	public static string ForChangePassword204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the password was changed.")
		.AppendLine()
		.ToString();
	public static string ForChangePassword404NotFound = new StringBuilder("**Not Found**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user does not exist.")
		.AppendLine()
		.ToString();
	public static string ForChangePassword409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the password is the same as the old.")
		.AppendLine()
		.ToString();
	
	public static string ForCreate200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have successfully created a user.")
		.AppendLine()
		.ToString();
	public static string ForCreate409Conflict = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user already exists.")
		.AppendLine()
		.ToString();
	
	public static string ForDelete204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user was deleted.")
		.AppendLine()
		.ToString();
	public static string ForDelete404NotFound = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user does not exist.")
		.AppendLine()
		.ToString();
	
	
	public static string ForUpdate204NoContent = new StringBuilder("**No Content**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user was updated.")
		.AppendLine()
		.ToString();
	public static string ForUpdate404NotFound = new StringBuilder("**Conflict**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the user does not exist.")
		.AppendLine()
		.ToString();
}
