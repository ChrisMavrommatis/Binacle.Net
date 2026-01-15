using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Exceptions;
using Binacle.Net.TestsKernel.Models;
using Bogus;

namespace Binacle.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class CreationTests : IClassFixture<CommonTestingFixture>
{
	private readonly CommonTestingFixture fixture;
	private readonly Bogus.Faker<TestItem> testItemsFaker;
	private readonly Bogus.Faker<TestBin> testBinsFaker;

	public CreationTests(CommonTestingFixture fixture)
	{
		this.fixture = fixture;
		Randomizer.Seed = new Random(605080);
		this.testItemsFaker = new Bogus.Faker<TestItem>()
			.RuleFor(x => x.Length, x => x.Random.Number(1, 65535))
			.RuleFor(x => x.Width, x => x.Random.Number(1, 65535))
			.RuleFor(x => x.Height, x => x.Random.Number(1, 65535));
		
		this.testBinsFaker = new Bogus.Faker<TestBin>()
			.RuleFor(x => x.Length, x => x.Random.Number(1, 65535))
			.RuleFor(x => x.Width, x => x.Random.Number(1, 65535))
			.RuleFor(x => x.Height, x => x.Random.Number(1, 65535));
	}


	[Fact(DisplayName = "Create With Null Bin Throws ArgumentNullException")]
	public void OnCreate_WithNullBin_Throws_ArgumentNullException()
	{
		var testItems = this.testItemsFaker.Generate(2);

		foreach(var (algorithmKey, algorithmFactory) in this.fixture.FittingAlgorithmsUnderTest)
		{
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(default!, testItems);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.AlgorithmsUnderTest)
		{
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(default!, testItems);
			});
		}
	}

	[Fact(DisplayName = "Create With Null Or Empty Items Throws ArgumentNullException")]
	public void Create_WithNullOrEmptyItems_Throws_ArgumentNullException()
	{
		var bin = this.testBinsFaker.Generate(1).FirstOrDefault()!;

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.FittingAlgorithmsUnderTest)
		{
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, default!);
			});
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, Enumerable.Empty<TestItem>().ToList());
			});

		}

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.AlgorithmsUnderTest)
		{
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, default!);
			});
			Should.Throw<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, Enumerable.Empty<TestItem>().ToList());
			});
		}
	}

	[Fact(DisplayName = "Create With 0 Dimension on Bins Throws DimensionException")]
	public void Create_With0DimensionOnBins_Throws_DimensionException()
	{
		var testItems = this.testItemsFaker.Generate(2);

		var binWith0Dimension = this.testBinsFaker.Generate(1).FirstOrDefault()!;
		binWith0Dimension.Width = 0;

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.FittingAlgorithmsUnderTest)
		{
			Should.Throw<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(binWith0Dimension, testItems);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.AlgorithmsUnderTest)
		{
			Should.Throw<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(binWith0Dimension, testItems);
			});
		}
		
	}

	[Fact(DisplayName = "Create With 0 Dimension on Items Throws DimensionException")]
	public void Create_With0DimensionOnItems_Throws_DimensionException()
	{
		var testItemsWith0Dimension = this.testItemsFaker
			.FinishWith((faker, item) => item.Width = 0)
			.Generate(2);

		var bin = this.testBinsFaker.Generate(1).FirstOrDefault()!;
		
		foreach (var (algorithmKey, algorithmFactory) in this.fixture.FittingAlgorithmsUnderTest)
		{
			Should.Throw<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, testItemsWith0Dimension);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.fixture.AlgorithmsUnderTest)
		{
			Should.Throw<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, testItemsWith0Dimension);
			});
		}
	}
}
