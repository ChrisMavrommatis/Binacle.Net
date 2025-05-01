using System.ComponentModel.DataAnnotations;
using Binacle.Net.ServiceModule.Domain.Common.Models;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class PagedResponse<T>
{
	public required int Total { get; set; }
	public required int Page { get; set; }
	public required int PageSize { get; set; }
	public required int TotalPages { get; set; }
	
	[Required]
	public List<T> Items { get; set; } = new List<T>();

}
