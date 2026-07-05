namespace Binacle.Lib.Abstractions.Models;

// Non-generic int shortcut. Adds the settable members over the read-only int shortcut.
public interface IWithDimensions : IWithReadOnlyDimensions
{
	new int Length { get; set; }
	new int Width { get; set; }
	new int Height { get; set; }
}
