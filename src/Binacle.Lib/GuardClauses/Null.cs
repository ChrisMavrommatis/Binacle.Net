using System.Runtime.CompilerServices;

namespace Binacle.Lib.GuardClauses;

internal static partial class GuardClauseExtensions
{
	public static IGuardClause Null<T>(
		this IGuardClause guardClause,
		T item, 
		[CallerArgumentExpression("item")] string? parameterName = null)
	{
		if (item is null)
		{
			throw new ArgumentNullException($"{parameterName} is null. {parameterName} is required");
		}
		return guardClause;
	}
}
