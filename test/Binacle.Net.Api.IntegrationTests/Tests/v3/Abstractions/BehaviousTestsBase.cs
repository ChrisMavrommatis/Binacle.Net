using System.Net.Http.Json;

namespace Binacle.Net.Api.IntegrationTests.v3.Abstractions;

public abstract partial class BehaviourTestsBase
{
	protected readonly BinacleApiFactory Sut;

	public BehaviourTestsBase(BinacleApiFactory sut)
	{
		this.Sut = sut;
	}

	protected async Task Request_Returns_200Ok<TRequest>(
		string url,
		TRequest request
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}

	protected async Task Request_Returns_400BadRequest<TRequest>(
		string url,
		TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}

	protected async Task Request_Returns_404NotFound<TRequest>(
		string url,
		TRequest request
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}
	
	protected async Task PackRequest_ValidateBasedOnParameters(
		string url,
		Api.v3.Requests.CustomPackRequest request,
		Action<Api.v3.Responses.PackResponse>? additionalValidation = null
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		var result = await response.Content
			.ReadFromJsonAsync<Api.v3.Responses.PackResponse>(this.Sut.JsonSerializerOptions);

		result.ShouldNotBeNull();
		result!.Data.ShouldNotBeEmpty();
		foreach (var binPackResult in result.Data)
		{
			binPackResult.Bin.ShouldNotBeNull();
			if (binPackResult.Result == Api.v3.Models.BinPackResultStatus.FullyPacked)
			{
				binPackResult.PackedItems.ShouldNotBeEmpty();
				binPackResult.UnpackedItems.ShouldBeNullOrEmpty();
			}
			else
			{
				binPackResult.UnpackedItems.ShouldNotBeEmpty();
			}
		}

		if (additionalValidation is not null)
		{
			additionalValidation.Invoke(result);
		}
	}
}
