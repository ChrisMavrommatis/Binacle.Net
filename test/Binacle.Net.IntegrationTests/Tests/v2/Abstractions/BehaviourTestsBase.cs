using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v2.Responses;

namespace Binacle.Net.IntegrationTests.v2.Abstractions;

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
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	protected async Task Request_Returns_400BadRequest<TRequest>(
		string url,
		TRequest request)
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}


	protected async Task FitRequest_ValidateBasedOnParameters<TRequest>(
		string url,
		TRequest request,
		Action<FitResponse>? additionalValidation = null
	)
		where TRequest : class, IWithFitRequestParameters
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content
			.ReadFromJsonAsync<FitResponse>(this.Sut.JsonSerializerOptions);

		result.ShouldNotBeNull();
		result!.Data.ShouldNotBeEmpty();
		if (request.Parameters?.FindSmallestBinOnly ?? false)
		{
			result.Data.ShouldHaveSingleItem();
		}

		foreach (var binFitResult in result.Data)
		{
			binFitResult.Bin.ShouldNotBeNull();
			if (request.Parameters!.ReportFittedItems ?? false)
			{
				binFitResult.FittedItems.ShouldNotBeEmpty();
			}
			else
			{
				binFitResult.FittedItems.ShouldBeNullOrEmpty();
			}

			if (request.Parameters!.ReportUnfittedItems ?? false)
			{
				binFitResult.UnfittedItems.ShouldNotBeEmpty();
			}
			else
			{
				binFitResult.UnfittedItems.ShouldBeNullOrEmpty();
			}
		}

		if (additionalValidation is not null)
		{
			additionalValidation.Invoke(result);
		}
	}

	protected async Task PackRequest_ValidateBasedOnParameters<TRequest>(
		string url,
		TRequest request,
		Action<PackResponse>? additionalValidation = null
	)
		where TRequest : class, IWithPackRequestParameters
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content
			.ReadFromJsonAsync<PackResponse>(
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);

		result.ShouldNotBeNull();
		result!.Data.ShouldNotBeEmpty();
		if (request.Parameters?.StopAtSmallestBin ?? false)
		{
			result.Data.ShouldHaveSingleItem();
		}

		foreach (var binPackResult in result.Data)
		{
			binPackResult.Bin.ShouldNotBeNull();
			if (binPackResult.Result == BinPackResultStatus.FullyPacked)
			{
				binPackResult.PackedItems.ShouldNotBeEmpty();
			}
			
			if (request.Parameters!.ReportPackedItemsOnlyWhenFullyPacked ?? false)
			{
				if (binPackResult.Result != BinPackResultStatus.FullyPacked)
				{
					binPackResult.PackedItems.ShouldBeNullOrEmpty();
				}
			}


			if (request.Parameters!.OptInToEarlyFails ?? false)
			{
				if (binPackResult.Result == BinPackResultStatus.EarlyFail_ContainerVolumeExceeded
				    || binPackResult.Result == BinPackResultStatus.EarlyFail_ContainerDimensionExceeded)
				{
					binPackResult.PackedItems.ShouldBeNullOrEmpty();
				}
			}
			

			if ((request.Parameters!.NeverReportUnpackedItems ?? false) ||
			    binPackResult.Result == BinPackResultStatus.FullyPacked)
			{
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
