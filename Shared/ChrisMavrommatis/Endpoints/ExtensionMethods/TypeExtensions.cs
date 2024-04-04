namespace ChrisMavrommatis.Endpoints.ExtensionMethods;

internal static class TypeExtensions
{
	internal static IEnumerable<Type> GetBaseTypesAndThis(this Type type)
	{
		Type? current = type;
		while (current != null)
		{
			yield return current;
			current = current.BaseType;
		}
	}
}
