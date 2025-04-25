namespace Binacle.Net.Kernel.Endpoints;

public class PagedList<T> : List<T>
{
	public int TotalCount { get; set; }
	public int PageSize { get; set; }
	public int PageNumber { get; set; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

	public PagedList(IEnumerable<T> items, int totalCount, int pageSize, int pageNumber)
	{
		this.AddRange(items);
		this.TotalCount = totalCount;
		this.PageSize = pageSize;
		this.PageNumber = pageNumber;
	}
}
