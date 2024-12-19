using System.Net.Http.Json;
using FluentAssertions;

namespace Binacle.Net.Api.IntegrationTests.v2.Abstractions;

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

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
	}


	protected async Task FitRequest_ValidateBasedOnParameters<TRequest>(
		string url,
		TRequest request,
		Action<Api.v2.Responses.FitResponse>? additionalValidation = null
	)
		where TRequest : class, Api.v2.Requests.IWithFitRequestParameters
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var result = await response.Content
			.ReadFromJsonAsync<Api.v2.Responses.FitResponse>(this.Sut.JsonSerializerOptions);

		result.Should().NotBeNull();
		result!.Data.Should().NotBeNullOrEmpty();
		if (request.Parameters?.FindSmallestBinOnly ?? false)
		{
			result.Data.Should().HaveCount(1);
		}

		foreach (var binFitResult in result.Data)
		{
			binFitResult.Bin.Should().NotBeNull();
			if (request.Parameters!.ReportFittedItems ?? false)
			{
				binFitResult.FittedItems.Should().NotBeNullOrEmpty();
			}
			else
			{
				binFitResult.FittedItems.Should().BeNullOrEmpty();
			}

			if (request.Parameters!.ReportUnfittedItems ?? false)
			{
				binFitResult.UnfittedItems.Should().NotBeNullOrEmpty();
			}
			else
			{
				binFitResult.UnfittedItems.Should().BeNullOrEmpty();
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
		Action<Api.v2.Responses.PackResponse>? additionalValidation = null
	)
		where TRequest : class, Api.v2.Requests.IWithPackRequestParameters
	{
		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions
		);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var result = await response.Content
			.ReadFromJsonAsync<Api.v2.Responses.PackResponse>(this.Sut.JsonSerializerOptions);

		result.Should().NotBeNull();
		result!.Data.Should().NotBeNullOrEmpty();
		if (request.Parameters?.StopAtSmallestBin ?? false)
		{
			result.Data.Should().HaveCount(1);
		}

		foreach (var binPackResult in result.Data)
		{
			binPackResult.Bin.Should().NotBeNull();
			if (binPackResult.Result == Api.v2.Models.BinPackResultStatus.FullyPacked)
			{
				binPackResult.PackedItems.Should().NotBeNullOrEmpty();
			}
			
			if (request.Parameters!.ReportPackedItemsOnlyWhenFullyPacked ?? false)
			{
				if (binPackResult.Result != Api.v2.Models.BinPackResultStatus.FullyPacked)
				{
					binPackResult.PackedItems.Should().BeNullOrEmpty();
				}
			}


			if (request.Parameters!.OptInToEarlyFails ?? false)
			{
				if (binPackResult.Result == Api.v2.Models.BinPackResultStatus.EarlyFail_ContainerVolumeExceeded
				    || binPackResult.Result == Api.v2.Models.BinPackResultStatus.EarlyFail_ContainerDimensionExceeded)
				{
					binPackResult.PackedItems.Should().BeNullOrEmpty();
				}
			}
			

			if ((request.Parameters!.NeverReportUnpackedItems ?? false) ||
			    binPackResult.Result == Api.v2.Models.BinPackResultStatus.FullyPacked)
			{
				binPackResult.UnpackedItems.Should().BeNullOrEmpty();
			}
			else
			{
				binPackResult.UnpackedItems.Should().NotBeNullOrEmpty();
			}
			
			
		}

		if (additionalValidation is not null)
		{
			additionalValidation.Invoke(result);
		}
	}
}
