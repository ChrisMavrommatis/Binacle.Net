using Binacle.Lib.Abstractions.Models;
using System.Runtime.CompilerServices;
using Binacle.Lib.Exceptions;

namespace Binacle.Lib.GuardClauses;

internal static partial class GuardClauseExtensions
{
	public static IGuardClause ZeroOrNegativeDimensions<T>(
		this IGuardClause guardClause,
		T item,
		[CallerArgumentExpression("item")] string? parameterName = null
	)
		where T : IWithReadOnlyDimensions
	{
		if (item.Length <= 0)
		{
			throw new DimensionException("Length", parameterName);
		}
		if (item.Width <= 0)
		{
			throw new DimensionException("Width", parameterName);
		}
		if (item.Height <= 0)
		{
			throw new DimensionException("Height", parameterName);
		}

		return guardClause;
	}
}
