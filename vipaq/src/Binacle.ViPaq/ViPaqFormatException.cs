namespace Binacle.ViPaq;

// A blob that does not conform to PROTOCOL.md — a bad header, a truncated body, or bytes left over. Every
// decode rejection in PROTOCOL.md §8 raises this. Encode rejections are argument errors instead: they mean
// the caller passed something the format cannot hold, which is a bug on their side, not bad input.
public sealed class ViPaqFormatException : Exception
{
	public ViPaqFormatException(string message) : base(message)
	{
	}

	public ViPaqFormatException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
