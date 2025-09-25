namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class PagedResponse<T>
{
	public required int Total { get; set; }
	public required int Page { get; set; }
	public required int PageSize { get; set; }
	public required int TotalPages { get; set; }
	public required List<T> Items { get; set; }
}
