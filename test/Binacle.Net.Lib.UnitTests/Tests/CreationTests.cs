using AutoFixture;
using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class CreationTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public Fixture AutoFixture { get; }

	

	public CreationTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.AutoFixture = new Fixture();
		
	}

	[Fact(DisplayName = "Create With Null Bin Throws ArgumentNullException")]
	public void OnCreate_WithNullBin_Throws_ArgumentNullException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		foreach(var fittingAlgorithm in this.Fixture.TestedFittingAlgorithms)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = fittingAlgorithm(null, testItems);
			});
		}

		foreach (var packingAlgorithm in this.Fixture.TestedPackingAlgorithms)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = packingAlgorithm(null, testItems);
			});
		}
	}

	[Fact(DisplayName = "Create With Null Or Empty Items Throws ArgumentNullException")]
	public void Create_WithNullOrEmptyItems_Throws_ArgumentNullException()
	{
		var bin = AutoFixture.Create<TestBin>();

		foreach (var fittingAlgorithm in this.Fixture.TestedFittingAlgorithms)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = fittingAlgorithm(bin, null);
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = fittingAlgorithm(bin, Enumerable.Empty<TestItem>());
			});

		}

		foreach (var packingAlgorithm in this.Fixture.TestedPackingAlgorithms)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = packingAlgorithm(bin, null);
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = packingAlgorithm(bin, Enumerable.Empty<TestItem>().ToList());
			});
		}
	}

	[Fact(DisplayName = "Create With 0 Dimension on Bins Throws DimensionException")]
	public void Create_With0DimensionOnBins_Throws_DimensionException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		var binWith0Dimension = AutoFixture.Build<TestBin>()
		   .With(x => x.Width, 0)
		   .Create();

		foreach (var fittingAlgorithm in this.Fixture.TestedFittingAlgorithms)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = fittingAlgorithm(binWith0Dimension, testItems);
			});
		}

		foreach (var packingAlgorithm in this.Fixture.TestedPackingAlgorithms)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = packingAlgorithm(binWith0Dimension, testItems);
			});
		}
		
	}

	[Fact(DisplayName = "Create With 0 Dimension on Items Throws DimensionException")]
	public void Create_With0DimensionOnItems_Throws_DimensionException()
	{
		var bin = AutoFixture.Create<TestBin>();

		var testItemsWith0Dimension = AutoFixture.Build<TestItem>()
		  .With(x => x.Width, 0)
		  .CreateMany(2)
		  .ToList();

		foreach (var fittingAlgorithm in this.Fixture.TestedFittingAlgorithms)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = fittingAlgorithm(bin, testItemsWith0Dimension);
			});
		}

		foreach (var packingAlgorithm in this.Fixture.TestedPackingAlgorithms)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = packingAlgorithm(bin, testItemsWith0Dimension);
			});
		}
	}
}
