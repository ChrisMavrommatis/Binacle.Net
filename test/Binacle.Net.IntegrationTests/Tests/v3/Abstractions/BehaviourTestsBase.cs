using System.Net.Http.Json;
using Binacle.Net.v3.Contracts;

namespace Binacle.Net.IntegrationTests.v3.Abstractions;

public abstract partial class BehaviourTestsBase
{
	protected readonly BinacleApi Sut;

	public BehaviourTestsBase(BinacleApi sut)
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
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}

	protected async Task Request_Returns_422UnprocessableContent<TRequest>(
		string url,
		TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	protected async Task Request_Returns_404NotFound<TRequest>(
		string url,
		TRequest request
	)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}
	
	protected async Task PackRequest_ValidateBasedOnParameters<TRequest>(
		string url,
		TRequest request,
		Action<Binacle.Net.v3.Contracts.PackResponse>? additionalValidation = null
	) 
		where TRequest : class, IWithPackingParameters
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<Binacle.Net.v3.Contracts.PackResponse>(
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();
		result!.Data.ShouldNotBeEmpty();
		foreach (var binPackResult in result.Data)
		{
			binPackResult.Bin.ShouldNotBeNull();
			if (binPackResult.Result == BinPackResultStatus.FullyPacked)
			{
				binPackResult.PackedItems.ShouldNotBeEmpty();
			}
			
			if (request.Parameters.ReportPackedItemsOnlyWhenFullyPacked)
			{
				if (binPackResult.Result != BinPackResultStatus.FullyPacked)
				{
					binPackResult.PackedItems.ShouldBeNullOrEmpty();
				}
			}


			if (request.Parameters.OptInToEarlyFails)
			{
				if (binPackResult.Result == BinPackResultStatus.EarlyFail_ContainerVolumeExceeded
				    || binPackResult.Result == BinPackResultStatus.EarlyFail_ContainerDimensionExceeded)
				{
					binPackResult.PackedItems.ShouldBeNullOrEmpty();
				}
			}
			

			if ((request.Parameters.NeverReportUnpackedItems) ||
			    binPackResult.Result == BinPackResultStatus.FullyPacked)
			{
				binPackResult.UnpackedItems.ShouldBeNullOrEmpty();
			}
			else
			{
				binPackResult.UnpackedItems.ShouldNotBeEmpty();
			}

			if (request.Parameters.IncludeViPaqData && binPackResult.PackedItems?.Count > 0)
			{
				binPackResult.ViPaqData.ShouldNotBeNullOrEmpty();
			}
			else
			{
				binPackResult.ViPaqData.ShouldBeNullOrEmpty();
			}
		}

		if (additionalValidation is not null)
		{
			additionalValidation.Invoke(result);
		}
	}
}
