using AutoFixture;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BehavioralTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public Fixture AutoFixture { get; }

	public BehavioralTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
		AutoFixture = new Fixture();
	}

	[Fact(DisplayName = "Create With Null Bin Throws ArgumentNullException")]
	public void OnCreate_WithNullBin_Throws_ArgumentNullException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(null, testItems);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(null, testItems);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(null, testItems);
		});

	}

	[Fact(DisplayName = "Create With Null Or Empty Items Throws ArgumentNullException")]
	public void Create_WithNullOrEmptyItems_Throws_ArgumentNullException()
	{
		var bin = AutoFixture.Create<TestBin>();

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, null);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, null);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, null);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, Enumerable.Empty<TestItem>());
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, Enumerable.Empty<TestItem>());
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, Enumerable.Empty<TestItem>().ToList());
		});
	}

	[Fact(DisplayName = "Create With 0 Dimension on Bins Throws DimensionException")]
	public void Create_With0DimensionOnBins_Throws_DimensionException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		var binWith0Dimension = AutoFixture.Build<TestBin>()
		   .With(x => x.Width, 0)
		   .Create();

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(binWith0Dimension, testItems);
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(binWith0Dimension, testItems);
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(binWith0Dimension, testItems);
		});
	}

	[Fact(DisplayName = "Create With 0 Dimension on Items Throws DimensionException")]
	public void Create_With0DimensionOnItems_Throws_DimensionException()
	{
		var bin = AutoFixture.Create<TestBin>();

		var testItemsWith0Dimension = AutoFixture.Build<TestItem>()
		  .With(x => x.Width, 0)
		  .CreateMany(2)
		  .ToList();

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, testItemsWith0Dimension);
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, testItemsWith0Dimension);
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithmInstance = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, testItemsWith0Dimension);
		});
	}
}
