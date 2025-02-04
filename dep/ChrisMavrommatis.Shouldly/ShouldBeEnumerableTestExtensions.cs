using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Shouldly;

[DebuggerStepThrough]
[ShouldlyMethods]
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class ShouldBeEnumerableTestExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldBeNullOrEmpty<T>([NotNull] this IEnumerable<T>? actual, string? customMessage = null)
    {
        if (actual?.Any() ?? false)
            throw new ShouldAssertException(new ExpectedShouldlyMessage(actual, customMessage).ToString());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<T>([NotNull] this IEnumerable<T>? actual, int count, string? customMessage = null)
    {
	    if (actual is null || actual.Count() != count)
		    throw new ShouldAssertException(new ExpectedShouldlyMessage(actual, customMessage).ToString());
    }

}
