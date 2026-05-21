using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.IntegrationTests.v4.Abstractions;

public abstract partial class BehaviourTestsBase
{
	protected readonly BinacleApi Sut;

	protected BehaviourTestsBase(BinacleApi sut)
	{
		this.Sut = sut;
	}

	protected async Task Request_Returns_200Ok<TRequest>(string url, TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	protected async Task Request_Returns_422UnprocessableContent<TRequest>(string url, TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	protected async Task Request_Returns_404NotFound<TRequest>(string url, TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	protected async Task FitRequest_Validate<TRequest>(
		string url,
		TRequest request,
		Action<FitBinResponse>? additionalValidation = null
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<FitBinResponse>(
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.AlgorithmUsed.ShouldNotBeNullOrEmpty();
		result.PackedItems.ShouldNotBeNull();
		result.UnpackedItems.ShouldNotBeNull();

		additionalValidation?.Invoke(result);
	}

	protected async Task PackRequest_Validate<TRequest>(
		string url,
		TRequest request,
		Action<PackBinResponse>? additionalValidation = null
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<PackBinResponse>(
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.AlgorithmUsed.ShouldNotBeNullOrEmpty();
		result.PackedItems.ShouldNotBeNull();
		result.UnpackedItems.ShouldNotBeNull();

		if (result.Status == BinPackResultStatus.FullyPacked)
		{
			result.PackedItems.ShouldNotBeEmpty();
			result.UnpackedItems.ShouldBeNullOrEmpty();
		}
		else
		{
			result.UnpackedItems.ShouldNotBeEmpty();
		}

		additionalValidation?.Invoke(result);
	}
}
