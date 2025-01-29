using System.Collections;

namespace Binacle.Net.Api.UIModule.ViewModels;

public class Errors  : IEnumerable<string>
{
	private List<string> _errors = new();
	
	public void Add(string error)
	{
		this._errors.Add(error);
	}
	
	public void Clear()
	{
		this._errors.Clear();
	}

	public IEnumerator<string> GetEnumerator()
	{
		return this._errors.GetEnumerator();
	}
	
	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
