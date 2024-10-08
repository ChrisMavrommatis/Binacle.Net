using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.GuardClauses;

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
