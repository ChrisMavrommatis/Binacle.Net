using System.Collections;

namespace Binacle.TestsKernel.Providers;

public class CustomProblemsScenarioNameProvider : IEnumerable<object[]>
{
	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in CustomProblemsScenarioProvider.GetScenarioNames())
		{
			yield return new object[] { scenario };
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
