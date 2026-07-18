namespace Binacle.Net.Kernel.OpenApi.Attributes;

// Marks a type whose non-nullable properties are always present in the response, so the schema should list them as
// `required`. Read by RequiredNonNullableSchemaTransformer. Generic: the Kernel carries the mechanism, the contract
// decides which types opt in. Inherited, so a base response type covers its derivations.
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class OpenApiRequireNonNullableAttribute : Attribute
{
}
