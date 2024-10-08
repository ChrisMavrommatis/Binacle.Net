namespace Binacle.Net.Lib.Exceptions;

public class DimensionException : Exception
{
	public string PropertyName { get; }
	public string? ParameterName { get; }

	public DimensionException(string propName, string? parameterName) : base($"{propName} on {parameterName ?? "N/A"} is a dimension and cannot be less than or equal to 0")
	{
		this.PropertyName = propName;
		this.ParameterName = parameterName;
	}

	public DimensionException(string propName, string parameterName, string message) : base(message)
	{
		this.PropertyName = propName;
		this.ParameterName = parameterName;
	}

	public DimensionException(string propName, string parameterName, string message, Exception inner) : base(message, inner)
	{
		this.PropertyName = propName;
		this.ParameterName = parameterName;
	}
}
