using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class ListAccountsResponse 
{
	public int Total { get; set; }
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
	public List<GetAccountResponse> Accounts { get; set; } = new List<GetAccountResponse>();
	public static ListAccountsResponse From(PagedList<Account> accounts)
	{
		var response = new ListAccountsResponse()
		{
			Total = accounts.TotalCount,
			Page = accounts.PageNumber,
			PageSize = accounts.PageSize,
			TotalPages =  accounts.TotalPages
		};
		foreach (var account in accounts)
		{
			response.Accounts.Add(GetAccountResponse.From(account));
		}
		return response;
	}
}

internal class ListAccountsResponseExample : ISingleOpenApiExamplesProvider<ListAccountsResponse>
{
	public IOpenApiExample<ListAccountsResponse> GetExample()
	{
		return OpenApiExample.Create(
			"listAccountsResponse",
			"List Accounts Response",
			new ListAccountsResponse()
		);
	}
}

internal class ListAccountsErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
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
