using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.IntegrationTests.Infrastructure;

// WAL and busy_timeout are not Microsoft.Data.Sqlite connection-string keywords, so the SQLite provider sets
// them by PRAGMA on open. This pins that it does — but only when SQLite is the configured backend; on the
// Postgres/Azure runs no SqliteConnection is registered and the test skips.
public class SqliteConnectionPragmaTests
{
	private readonly BinacleApi sut;

	public SqliteConnectionPragmaTests(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Fact]
	public void Configured_sqlite_connection_has_wal_and_busy_timeout()
	{
		using var scope = this.sut.Services.CreateScope();
		var connection = scope.ServiceProvider.GetService<SqliteConnection>();
		Assert.SkipWhen(connection is null, "SQLite is not the configured backend for this run.");

		using var command = connection!.CreateCommand();

		command.CommandText = "PRAGMA journal_mode;";
		var journalMode = (string)command.ExecuteScalar()!;

		command.CommandText = "PRAGMA busy_timeout;";
		var busyTimeout = (long)command.ExecuteScalar()!;

		journalMode.ShouldBe("wal");
		busyTimeout.ShouldBe(5000);
	}
}
