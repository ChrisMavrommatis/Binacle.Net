namespace Binacle.Net.Kernel.OpenApi.Attributes;

// Documents an OpenAPI `minimum`/`maximum` on a property, with no runtime effect. The API accepts out-of-range
// values on purpose and rejects them through its own validators, so this describes the valid range only — it is
// never enforced at the framework level (unlike `[Range]`, which can also trigger built-in validation). Read by
// SchemaRangeSchemaTransformer. `NaN` means the bound is unset, so a property can set only a minimum or only a
// maximum.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OpenApiSchemaRangeAttribute : Attribute
{
	public double Minimum { get; set; } = double.NaN;
	public double Maximum { get; set; } = double.NaN;
}
