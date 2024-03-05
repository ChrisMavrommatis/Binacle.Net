using AutoFixture;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.Tests.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing.Tests;

public class BehavioralTests : IClassFixture<FirstFitDecreasingFixture>
{
	private FirstFitDecreasingFixture Fixture { get; }
	public Fixture AutoFixture { get; }

	public BehavioralTests(FirstFitDecreasingFixture fixture)
	{
		this.Fixture = fixture;
		this.AutoFixture = new Fixture();
	}

	[Fact]
	public void Build_WithNullOrEmptyBins_Throws_ArgumentNullException()
	{
		var testItems = this.AutoFixture.CreateMany<TestItem>(2);

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

	}

	[Fact]
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
	}

	[Fact]
	public void Build_With0DimensionBins_Throws_DimensionException()
	{
		var testItems = AutoFixture.CreateMany<TestItem>(2);

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
	}

	[Fact]
	public void Build_With0DimensionItems_Throws_DimensionException()
	{
		var testBins = AutoFixture.CreateMany<TestBin>(2);
		var testItemsWith0Dimension = AutoFixture.Build<TestItem>()
		  .With(x => x.Width, 0)
		  .CreateMany(2);

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
	}
}
