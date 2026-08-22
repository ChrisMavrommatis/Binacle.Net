using Microsoft.AspNetCore.Http;

namespace Binacle.Net.Kernel.UnitTests.Paths;

// Which paths must never answer with a web page. Every module declares its own, and the UI module reads the
// set per request - so a prefix that is declared but does not match is how an API route starts returning HTML.
[Trait("Behavioral Tests", "Ensures reserved path prefixes are declared and matched as expected")]
public class ReservedPathOptionsTests
{
	[Theory]
	[InlineData("/api")]
	[InlineData("/api/v3")]
	[InlineData("/api/v3/pack/by-custom")]
	public void Covers_A_Declared_Prefix_And_Everything_Under_It(string path)
	{
		var options = new ReservedPathOptions();
		options.AddPrefix("/api");

		var covered = options.Covers(path);

		covered.ShouldBeTrue();
	}

	// The whole reason this matches on segments: "/apidocs" starts with the characters of "/api" and is a
	// different endpoint.
	[Theory]
	[InlineData("/apidocs")]
	[InlineData("/api-docs")]
	[InlineData("/")]
	[InlineData("/packing")]
	public void Does_Not_Cover_A_Path_That_Only_Shares_The_Prefixs_Characters(string path)
	{
		var options = new ReservedPathOptions();
		options.AddPrefix("/api");

		var covered = options.Covers(path);

		covered.ShouldBeFalse();
	}

	[Fact]
	public void Matches_A_Declared_Prefix_Regardless_Of_Case()
	{
		var options = new ReservedPathOptions();
		options.AddPrefix("/api");

		var covered = options.Covers("/API/V4/presets");

		covered.ShouldBeTrue();
	}

	// The Diagnostics module declares a health path it reads out of configuration, so an unconfigured one
	// arrives here as null. Held rather than ignored, an empty prefix would reserve every path on the host and
	// the demo UI would stop rendering entirely.
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void An_Empty_Prefix_Is_Ignored_Rather_Than_Reserving_Everything(string? prefix)
	{
		var options = new ReservedPathOptions();

		options.AddPrefix(prefix!);

		options.Prefixes.ShouldBeEmpty();
		options.Covers("/").ShouldBeFalse();
		options.Covers("/packing").ShouldBeFalse();
	}

	[Fact]
	public void The_Same_Prefix_Declared_Twice_Is_Held_Once()
	{
		var options = new ReservedPathOptions();

		options.AddPrefix("/api");
		options.AddPrefix("/API");

		options.Prefixes.Count.ShouldBe(1);
	}

	[Fact]
	public void Covers_Nothing_When_No_Module_Declared_A_Prefix()
	{
		var options = new ReservedPathOptions();

		var covered = options.Covers("/api/v4/presets");

		covered.ShouldBeFalse();
	}

	// The set the shipped image actually declares: four from Program.cs, one from the UI module, two from
	// Diagnostics. A page route must fall through all of them.
	[Theory]
	[InlineData("/api/v4/presets", true)]
	[InlineData("/openapi/v4.json", true)]
	[InlineData("/swagger/index.html", true)]
	[InlineData("/scalar", true)]
	[InlineData("/_content/Binacle.Net.UIModule/js/packing_demo.js", true)]
	[InlineData("/health", true)]
	[InlineData("/debug/request", true)]
	[InlineData("/packing", false)]
	[InlineData("/instance", false)]
	[InlineData("/error/404", false)]
	public void Each_Declared_Prefix_Is_Matched_Independently(string path, bool expected)
	{
		var options = new ReservedPathOptions();
		foreach (var prefix in new[] { "/api", "/openapi", "/swagger", "/scalar", "/_content", "/health", "/debug" })
		{
			options.AddPrefix(prefix);
		}

		var covered = options.Covers(path);

		covered.ShouldBe(expected);
	}

	// Covers matches PathString, which refuses anything without a leading slash. AddPrefix adds it, so a
	// module that declares "api" gets what it meant instead of a throw on every request.
	[Fact]
	public void A_Prefix_Without_A_Leading_Slash_Gains_One()
	{
		var options = new ReservedPathOptions();
		options.AddPrefix("api");

		var covered = options.Covers("/api/v4/presets");

		covered.ShouldBeTrue();
	}

	// The same normalized value is what the health check and the debug page print.
	[Fact]
	public void A_Normalized_Prefix_Is_Stored_With_Its_Slash()
	{
		var options = new ReservedPathOptions();
		options.AddPrefix("api");

		var prefixes = options.Prefixes;

		prefixes.ShouldContain("/api");
	}
}
