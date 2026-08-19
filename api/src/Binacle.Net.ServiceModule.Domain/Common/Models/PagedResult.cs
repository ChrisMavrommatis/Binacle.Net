namespace Binacle.Net.ServiceModule.Domain.Common.Models;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
