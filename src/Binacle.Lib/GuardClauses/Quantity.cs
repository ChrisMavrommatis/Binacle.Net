using Binacle.Lib.Abstractions.Models;
using System.Runtime.CompilerServices;
using Binacle.Lib.Exceptions;

namespace Binacle.Lib.GuardClauses;

internal static partial class GuardClauseExtensions
{
	public static IGuardClause ZeroOrNegativeQuantity<T>(
		this IGuardClause guardClause,
		T item,
		[CallerArgumentExpression("item")] string? parameterName = null
	)
		where T : IWithQuantity
	{
		if (item.Quantity <= 0)
		{
			throw new DimensionException("Quantity", parameterName);
		}
		return guardClause;
	}
}
