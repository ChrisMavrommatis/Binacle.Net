using System.Collections;

namespace Binacle.TestsKernel.Providers;

public class BischoffSuiteScenarioNameProvider : IEnumerable<object[]>
{
	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in BischoffSuiteScenarioProvider.GetScenarioNames())
		{
			yield return new object[] { scenario };
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
