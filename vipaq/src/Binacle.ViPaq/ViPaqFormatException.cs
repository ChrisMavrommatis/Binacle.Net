namespace Binacle.ViPaq;

// A blob that does not conform to PROTOCOL.md: a bad header, a truncated body, or bytes left over. Every decode
// rejection in §8 raises this. Encode rejections are argument errors instead - the caller's bug, not bad input.
public sealed class ViPaqFormatException : Exception
{
	public ViPaqFormatException(string message) : base(message)
	{
	}

	public ViPaqFormatException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
