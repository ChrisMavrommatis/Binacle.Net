using System.Text.Json.Serialization;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class GetSubscriptionResponse
{
	public Guid Id { get; set; }
	public Guid AccountId { get; set; }
	
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public SubscriptionType Type { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public SubscriptionStatus Status { get; set; }
	
	public DateTimeOffset CreatedAtUtc { get; set; }
	
	public static GetSubscriptionResponse From(Subscription subscription)
	{
		return new GetSubscriptionResponse
		{
			Id = subscription.Id,
			AccountId = subscription.AccountId,
			Type = subscription.Type,
			Status = subscription.Status,
			CreatedAtUtc = subscription.CreatedAtUtc
		};
	}
	internal class Example : ISingleOpenApiExamplesProvider<GetSubscriptionResponse>
	{
		public IOpenApiExample<GetSubscriptionResponse> GetExample()
		{
			return OpenApiExample.Create(
				"getSubscriptionResponse",
				"Get Subscription Response",
				new GetSubscriptionResponse()
				{
					Id = Guid.Parse("7C8C06A8-6CD2-441F-AD3C-D25E1AAE1520"),
					AccountId =  Guid.Parse("B0FAE205-ADDF-4404-B5D8-5A3D8B016362"),
					Type = SubscriptionType.Normal,					
					Status = SubscriptionStatus.Active,
					CreatedAtUtc = new DateTimeOffset(2025, 1, 11, 14, 32, 41, TimeSpan.Zero),
				}
			);
		
		}
	}
	internal class ErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
	{
		public IOpenApiExample<ErrorResponse> GetExample()
		{
			return OpenApiExample.Create(
				"idparametererror",
				"ID Parameter Error",
				ErrorResponse.IdToGuidParameterError
			);
		}
	}

	
}
