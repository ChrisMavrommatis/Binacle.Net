namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApi Sut;
	protected readonly HttpClient Client;

	public AdminEndpointsTestsBase(BinacleApi sut)
	{
		this.Sut = sut;
		this.Client = sut.CreateClient();
	}

	public virtual ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public virtual ValueTask DisposeAsync()
	{
		this.Client.Dispose();
		return ValueTask.CompletedTask;
	}
	
	protected static Guid GetCreatedId(HttpResponseMessage response)
	{
		var location = response.Headers.Location!.ToString();
		var parts = location.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
		var id = Guid.Parse(parts.Last());
		return id;
	}
}
