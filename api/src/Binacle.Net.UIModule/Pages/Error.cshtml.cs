using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.UIModule.Pages;

internal class ErrorModel : PageModel
{
	private readonly IWebHostEnvironment environment;

	public ErrorModel(IWebHostEnvironment environment)
	{
		this.environment = environment;
	}

	public string Title { get; private set; } = "Error";
	public string Message { get; private set; } = "Something went wrong while handling your request.";
	public string? RequestId { get; private set; }

	public void OnGet(string? errorCode)
	{
		if (int.TryParse(errorCode, out var statusCode))
		{
			this.Title = $"Error {statusCode}";
			this.Message = MessageFor(statusCode);
		}

		// Only in development: the trace id is a detail of this instance, not something a visitor needs.
		if (this.environment.IsDevelopment())
		{
			this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
		}
	}

	private static string MessageFor(int statusCode) => statusCode switch
	{
		404 => "That page does not exist. Check the address, or start again from the home page.",
		403 => "You do not have access to that page.",
		500 => "Something went wrong on this instance. Try again, and check the server log if it keeps happening.",
		_ => "Something went wrong while handling your request."
	};
}
