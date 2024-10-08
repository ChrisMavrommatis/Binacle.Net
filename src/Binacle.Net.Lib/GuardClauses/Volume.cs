using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.GuardClauses;

internal static partial class GuardClauseExtensions
{
	public static IGuardClause ZeroOrNegativeVolume<T>(
		this IGuardClause guardClause,
		T item,
		[CallerArgumentExpression("item")] string? parameterName = null
	)
		where T : IWithReadOnlyVolume
	{
		if (item.Volume <= 0)
		{
			throw new DimensionException("Volume", parameterName);
		}
		return guardClause;
	}
}
