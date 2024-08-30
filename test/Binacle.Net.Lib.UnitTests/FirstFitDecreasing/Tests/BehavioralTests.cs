using AutoFixture;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BehavioralTests : IClassFixture<FirstFitDecreasingFixture>
{
	private FirstFitDecreasingFixture Fixture { get; }
	public Fixture AutoFixture { get; }

	public BehavioralTests(FirstFitDecreasingFixture fixture)
	{
		this.Fixture = fixture;
		this.AutoFixture = new Fixture();
	}

	[Fact(DisplayName = "Build With Null Or Empty Bins Throws ArgumentNullException")]
	public void Build_WithNullOrEmptyBins_Throws_ArgumentNullException()
	{
		var testItems = this.AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
			.WithBins((IEnumerable<TestBin>)null)
			.AndItems(testItems)
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
			.WithBins((IEnumerable<TestBin>)null)
			.AndItems(testItems)
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
			.WithBins(Enumerable.Empty<TestBin>())
			.AndItems(testItems)
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
			.WithBins(Enumerable.Empty<TestBin>())
			.AndItems(testItems)
			.Build();
		});


		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithm = new Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(null, testItems);
		});

	}

	[Fact(DisplayName = "Build With Null Or Empty Items Throws ArgumentNullException")]
	public void Build_WithNullOrEmptyItems_Throws_ArgumentNullException()
	{
		var testBins = this.AutoFixture.CreateMany<TestBin>(2);

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
			.WithBins(testBins)
			.AndItems((IEnumerable<TestItem>)null)
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
			.WithBins(testBins)
			.AndItems((IEnumerable<TestItem>)null)
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithm = new Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(
				testBins.First(),
				null
			);
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
			.WithBins(testBins)
			.AndItems(Enumerable.Empty<TestItem>())
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
			.WithBins(testBins)
			.AndItems(Enumerable.Empty<TestItem>())
			.Build();
		});

		Assert.Throws<ArgumentNullException>(() =>
		{
			var algorithm = new Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(
				testBins.First(),
				Enumerable.Empty<TestItem>().ToList()
			);
		});
	}

	[Fact(DisplayName = "Build With 0 Dimension on Bins Throws DimensionException")]
	public void Build_With0DimensionOnBins_Throws_DimensionException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2)
			.ToList();

		var testBinsWith0Dimension = AutoFixture.Build<TestBin>()
		   .With(x => x.Width, 0)
		   .CreateMany(2);

		Assert.Throws<DimensionException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
				.WithBins(testBinsWith0Dimension)
				.AndItems(testItems)
				.Build();
		});

		Assert.Throws<DimensionException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
				.WithBins(testBinsWith0Dimension)
				.AndItems(testItems)
				.Build();
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithm = new Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(
				testBinsWith0Dimension.First(),
				testItems
			);
		});
	}

	[Fact(DisplayName = "Build With 0 Dimension on Items Throws DimensionException")]
	public void Build_With0DimensionOnItems_Throws_DimensionException()
	{
		var testBins = AutoFixture.CreateMany<TestBin>(2)
			.ToList();
		var testItemsWith0Dimension = AutoFixture.Build<TestItem>()
		  .With(x => x.Width, 0)
		  .CreateMany(2)
		  .ToList();

		Assert.Throws<DimensionException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v1()
				.WithBins(testBins)
				.AndItems(testItemsWith0Dimension)
				.Build();
		});

		Assert.Throws<DimensionException>(() =>
		{
			var strategy = new Lib.Strategies.FirstFitDecreasing_v2()
				.WithBins(testBins)
				.AndItems(testItemsWith0Dimension)
				.Build();
		});

		Assert.Throws<DimensionException>(() =>
		{
			var algorithm = new Lib.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(
				testBins.First(),
				testItemsWith0Dimension
			);
		});
	}
}
