namespace Binacle.TestReporting;

// Names one output file and its heading. Tests that share a Filename are grouped into the same file, each
// as its own section.
public class ResultFile
{
	public required string Filename { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
}
