namespace Binacle.Net.Api.Kernel.Models;

public interface IConfigurationOptions
{
	static abstract string SectionName { get; }
	static abstract string FilePath { get; }
	static abstract bool Optional { get; }
	static abstract bool ReloadOnChange { get; }
	static abstract string? GetEnvironmentFilePath(string environment);
}
