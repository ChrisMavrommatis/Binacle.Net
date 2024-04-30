namespace ChrisMavrommatis.SwaggerExamples.Attributes;

public abstract class SwaggerExamplesAttribute : Attribute
{
	protected void GuardAgainstInvalidSetup(Type modelType, Type examplesProviderType)
	{
		if(
			!examplesProviderType.IsAssignableTo(typeof(SingleSwaggerExamplesProvider<>).MakeGenericType(modelType))
			&& !examplesProviderType.IsAssignableTo(typeof(MultipleSwaggerExamplesProvider<>).MakeGenericType(modelType))
			)
		{
			throw new ArgumentException($"{examplesProviderType.Name} must inherit from SingleSwaggerExamplesProvider<{modelType.Name}> or MultipleSwaggerExamplesProvider<{modelType.Name}>");
		}
	}
}
