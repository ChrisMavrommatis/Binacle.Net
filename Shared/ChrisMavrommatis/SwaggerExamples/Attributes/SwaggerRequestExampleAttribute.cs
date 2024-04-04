namespace ChrisMavrommatis.SwaggerExamples.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerRequestExampleAttribute : SwaggerExamplesAttribute
{
	public SwaggerRequestExampleAttribute(Type requestType, Type examplesProviderType)
	{
		this.GuardAgainstInvalidSetup(requestType, examplesProviderType);
		this.RequestType = requestType;
		this.ExamplesProviderType = examplesProviderType;
	}

	public Type ExamplesProviderType { get; }
	public Type RequestType { get; }
}
