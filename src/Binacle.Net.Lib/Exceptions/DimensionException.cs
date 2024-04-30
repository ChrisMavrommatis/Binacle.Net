namespace Binacle.Net.Lib.Exceptions;

public class DimensionException : Exception
{
	public string? PropertyName { get; }

	public DimensionException()
	{
	}

	public DimensionException(string propName) : base($"{propName} is a dimension and cannot be less than or equal to 0")
	{
		this.PropertyName = propName;
	}

	public DimensionException(string propName, string message) : base(message)
	{
		this.PropertyName = propName;
	}

	public DimensionException(string propName, string message, Exception inner) : base(message, inner)
	{
		this.PropertyName = propName;
	}
}
