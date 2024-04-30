using Binacle.Net.Lib.UnitTests.Data.Models;
using System.Collections;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal abstract class ScenarioTestDataProvider : IEnumerable<object[]>
{
	protected readonly Dictionary<string, Scenario> _scenarios;

	protected ScenarioTestDataProvider()
	{
		_scenarios = new Dictionary<string, Scenario>();
	}

	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in _scenarios.Values)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
