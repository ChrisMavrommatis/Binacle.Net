namespace Binacle.Net.Lib.GuardClauses;

internal interface IGuardClause
{
}

internal class Guard : IGuardClause
{
	public static IGuardClause Against { get; } = new Guard();

	private Guard() { }
}
