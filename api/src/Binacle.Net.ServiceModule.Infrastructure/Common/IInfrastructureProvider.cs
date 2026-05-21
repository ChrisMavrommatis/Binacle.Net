using Binacle.Net.Kernel.Configuration.Models;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.ServiceModule.Infrastructure.Common;

internal interface IInfrastructureProvider
{
	string ConnectionStringName { get; }
	void Register(IHostApplicationBuilder builder, ConnectionString connectionString);
}
