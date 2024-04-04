namespace ChrisMavrommatis.SwaggerExamples.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseExampleAttribute : SwaggerExamplesAttribute
{
	public SwaggerResponseExampleAttribute(Type responseType, Type examplesProviderType, int statusCode = 0)
	{
		this.GuardAgainstInvalidSetup(responseType, examplesProviderType);
		this.ResponseType = responseType;
		this.ExamplesProviderType = examplesProviderType;
		this.StatusCode = statusCode;
	}

	public Type ExamplesProviderType { get; }
	public Type ResponseType { get; }
	public int StatusCode { get; }
}
