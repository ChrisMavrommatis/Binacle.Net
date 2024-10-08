using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.GuardClauses;

internal static partial class GuardClauseExtensions
{
	public static IGuardClause NullOrEmpty<T>(
		this IGuardClause guardClause,
		IEnumerable<T> item,
		[CallerArgumentExpression("item")] string? parameterName = null)
	{
		if (item is null || !item.Any())
		{
			throw new ArgumentNullException($"{parameterName} is null or empty. At least one of {parameterName} is required");
		}
		return guardClause;
	}

	public static IGuardClause NullOrEmpty<T>(
		this IGuardClause guardClause,
		IList<T> item,
		[CallerArgumentExpression("item")] string? parameterName = null)
	{
		if (item is null || item.Count <= 0)
		{
			throw new ArgumentNullException($"{parameterName} is null or empty. At least one of {parameterName} is required");
		}
		return guardClause;
	}

	public static IGuardClause NullOrEmpty<T>(
		this IGuardClause guardClause,
		T[] item,
		[CallerArgumentExpression("item")] string? parameterName = null)
	{
		if (item is null || item.Length <= 0)
		{
			throw new ArgumentNullException($"{parameterName} is null or empty. At least one of {parameterName} is required");
		}
		return guardClause;
	}
}
