using Binacle.Lib.Abstractions.Models;
using System.Runtime.CompilerServices;
using Binacle.Lib.Exceptions;

namespace Binacle.Lib.GuardClauses;

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
