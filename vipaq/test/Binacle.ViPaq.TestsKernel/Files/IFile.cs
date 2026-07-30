namespace Binacle.ViPaq.TestsKernel.Files;

// An embedded packed-data file. The name carries three parts — <family>.<name>.<algorithm> — that the reader
// groups by; the extension is kept apart from them.
public interface IFile
{
	Stream OpenRead();
	string Family { get; }
	string Name { get; }
	string Algorithm { get; }
	string Extension { get; }
}
