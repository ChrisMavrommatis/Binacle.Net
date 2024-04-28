using Binacle.Net.Lib.Tests.Data.Providers;
using Binacle.Net.Lib.Tests.Models;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class FirstFitDecreasingFixture : IDisposable
{
	private readonly BinTestDataProvider binTestDataProvider;

	public Dictionary<string, List<TestBin>> Bins => this.binTestDataProvider.Bins;

	public FirstFitDecreasingFixture()
	{
		this.binTestDataProvider = new BinTestDataProvider(Data.Constants.SolutionRootBasePath);

	}

	public void Dispose()
	{

	}

}
