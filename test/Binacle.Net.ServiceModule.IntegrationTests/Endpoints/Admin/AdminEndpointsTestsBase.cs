using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
