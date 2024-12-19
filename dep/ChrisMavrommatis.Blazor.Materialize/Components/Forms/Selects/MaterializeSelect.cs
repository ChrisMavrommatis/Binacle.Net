using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ChrisMavrommatis.Blazor.Materialize.Components;



public class MaterializeSelect<TValue>  : MaterializeComponentBase
{
	[Parameter] 
	public string Id { get; set; } = string.Empty;
	
	[Parameter] 
	public string Label { get; set; } = string.Empty;
	
	[Parameter] 
	public TValue Value { get; set; }
	
	[Parameter] 
	public EventCallback<TValue> ValueChanged { get; set; }
	
	[Parameter]
	public Dictionary<TValue, string> Options { get; set; } = new();
	
	[Parameter]
	public Expression<Func<TValue>>? ValueExpression { get; set; }
	
	[Parameter] 
	public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
	
}
