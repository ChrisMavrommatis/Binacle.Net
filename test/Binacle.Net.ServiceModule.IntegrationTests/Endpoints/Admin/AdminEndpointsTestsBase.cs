namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApi Sut;
	
	public AdminEndpointsTestsBase(BinacleApi sut)
	{
		this.Sut = sut;
	}
	
	public virtual ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public virtual ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
	
	protected Guid GetCreatedId(HttpResponseMessage response)
	{
		var location = response.Headers.Location!.ToString();
		var parts = location.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
		var id = Guid.Parse(parts.Last());
		return id;
	}
}
