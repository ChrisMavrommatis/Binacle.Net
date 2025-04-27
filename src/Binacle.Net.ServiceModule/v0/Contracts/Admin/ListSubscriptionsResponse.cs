using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class ListSubscriptionsResponse 
{
	public int Total { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
	public List<GetSubscriptionResponse> Subscriptions { get; set; } = new List<GetSubscriptionResponse>();
	public static ListSubscriptionsResponse From(PagedList<Subscription> subscriptions)
	{
		var response = new ListSubscriptionsResponse()
		{
			Total = subscriptions.TotalCount,
			Page = subscriptions.PageNumber,
			PageSize = subscriptions.PageSize,
			TotalPages =  subscriptions.TotalPages
		};
		foreach (var subscription in subscriptions)
		{
			response.Subscriptions.Add(GetSubscriptionResponse.From(subscription));
		}
		return response;
	}
	
	internal class Example : ISingleOpenApiExamplesProvider<ListSubscriptionsResponse>
	{
		public IOpenApiExample<ListSubscriptionsResponse> GetExample()
		{
			return OpenApiExample.Create(
				"listSubscriptionsResponse",
				"List Subscriptions Response",
				new ListSubscriptionsResponse() // TODO
			);
		}
	}
	
	internal class ErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
	{
		public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"pageNumberError",
				"Page Number Error",
				ErrorResponse.PageNumberError
			);
			yield return OpenApiExample.Create(
				"pageSizeError",
				"Page Size Error",
				ErrorResponse.PageSizeError
			);
		
		}
	}
	
}
