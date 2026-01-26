namespace Binacle.TestsKernel;

public interface IFile
{
	Stream OpenRead();
	string Name { get; }
	string Extension { get; }
	string Path { get; }
	string Folder { get; }
}
