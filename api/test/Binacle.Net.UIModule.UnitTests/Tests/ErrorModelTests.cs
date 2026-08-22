using System.Diagnostics;
using Binacle.Net.UIModule.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Binacle.Net.UIModule.UnitTests;

// The page UseStatusCodePagesWithReExecute re-executes into. It is reached with whatever status the pipeline
// produced, including one nobody wrote a message for.
[Trait("Behavioral Tests", "Ensures the error page reports a status without leaking instance detail")]
public class ErrorModelTests
{
	private static ErrorModel ErrorPageIn(string environmentName)
	{
		var page = new ErrorModel(new FakeWebHostEnvironment(environmentName));
		page.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
		return page;
	}

	[Theory]
	[InlineData("404", "That page does not exist. Check the address, or start again from the home page.")]
	[InlineData("403", "You do not have access to that page.")]
	[InlineData("500", "Something went wrong on this instance. Try again, and check the server log if it keeps happening.")]
	public void A_Known_Status_Gets_Its_Own_Message(string errorCode, string expectedMessage)
	{
		var page = ErrorPageIn("Production");

		page.OnGet(errorCode);

		page.Title.ShouldBe($"Error {errorCode}");
		page.Message.ShouldBe(expectedMessage);
	}

	[Theory]
	[InlineData("418")]
	[InlineData("502")]
	public void A_Status_With_No_Message_Still_Names_Itself(string errorCode)
	{
		var page = ErrorPageIn("Production");

		page.OnGet(errorCode);

		page.Title.ShouldBe($"Error {errorCode}");
		page.Message.ShouldBe("Something went wrong while handling your request.");
	}

	// The route segment is optional and the re-execute is not the only way in - someone can open /error.
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("not-a-status")]
	public void A_Missing_Or_Unreadable_Status_Falls_Back_Without_Naming_One(string? errorCode)
	{
		var page = ErrorPageIn("Production");

		page.OnGet(errorCode);

		page.Title.ShouldBe("Error");
		page.Message.ShouldBe("Something went wrong while handling your request.");
	}

	// The trace id identifies this instance's request. It is a development aid and must not reach a visitor.
	[Theory]
	[InlineData("Production")]
	[InlineData("Staging")]
	[InlineData("Test")]
	public void The_Request_Id_Is_Withheld_Outside_Development(string environmentName)
	{
		var page = ErrorPageIn(environmentName);

		page.OnGet("500");

		page.RequestId.ShouldBeNull();
	}

	[Fact]
	public void The_Request_Id_Is_Shown_In_Development()
	{
		var page = ErrorPageIn("Development");

		page.OnGet("500");

		page.RequestId.ShouldBe(Activity.Current?.Id ?? page.HttpContext.TraceIdentifier);
		page.RequestId.ShouldNotBeNullOrWhiteSpace();
	}
}
