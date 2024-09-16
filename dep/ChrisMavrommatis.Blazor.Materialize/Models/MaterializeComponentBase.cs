using Microsoft.AspNetCore.Components;

namespace ChrisMavrommatis.Blazor.Materialize.Models;

public abstract class MaterializeComponentBase : ComponentBase
{
	[Parameter(CaptureUnmatchedValues = true)]
	public IReadOnlyDictionary<string, object> UnmatchedAttributes { get; set; } = new Dictionary<string, object>();

	protected IReadOnlyDictionary<string, object>? _attributes;
	protected string _classes = "";

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		// extract class attribute
		_classes = $"{UnmatchedAttributes.GetValueOrDefault("class")}";

		// extract non-class attributes
		_attributes =
		  UnmatchedAttributes
		  .Where(x => x.Key != "class")
		  .ToDictionary(k => k.Key, v => v.Value);
	}
}
