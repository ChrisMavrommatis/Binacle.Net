using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Binacle.Net.UIModule.UnitTests;

// The pages read EnvironmentName and nothing else. A null file provider is enough because none of them touch
// the file system.
internal sealed class FakeWebHostEnvironment : IWebHostEnvironment
{
	public FakeWebHostEnvironment(string environmentName)
	{
		this.EnvironmentName = environmentName;
	}

	public string EnvironmentName { get; set; }
	public string ApplicationName { get; set; } = "Binacle.Net";
	public string WebRootPath { get; set; } = string.Empty;
	public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
	public string ContentRootPath { get; set; } = string.Empty;
	public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
