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

		foreach(var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(default!, testItems);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(default!, testItems);
			});
		}
	}

	[Fact(DisplayName = "Create With Null Or Empty Items Throws ArgumentNullException")]
	public void Create_WithNullOrEmptyItems_Throws_ArgumentNullException()
	{
		var bin = AutoFixture.Create<TestBin>();

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, default!);
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, Enumerable.Empty<TestItem>().ToList());
			});

		}

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, default!);
			});
			Assert.Throws<ArgumentNullException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, Enumerable.Empty<TestItem>().ToList());
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

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(binWith0Dimension, testItems);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(binWith0Dimension, testItems);
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

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.FittingAlgorithmsUnderTest)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, testItemsWith0Dimension);
			});
		}

		foreach (var (algorithmKey, algorithmFactory) in this.Fixture.PackingAlgorithmsUnderTest)
		{
			Assert.Throws<DimensionException>(() =>
			{
				var algorithmInstance = algorithmFactory(bin, testItemsWith0Dimension);
			});
		}
	}
}
