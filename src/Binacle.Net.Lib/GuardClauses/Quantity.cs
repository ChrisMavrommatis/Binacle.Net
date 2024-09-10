using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.GuardClauses;

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
