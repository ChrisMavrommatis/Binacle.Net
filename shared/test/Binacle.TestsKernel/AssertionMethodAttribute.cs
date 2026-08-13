namespace Binacle.TestsKernel;

// Marks a helper that does the asserting on a test's behalf.
//
// Sonar's S2699 only walks the test method body, so a test that asserts one call away reads as assertion-free
// and gets flagged. The analyser matches this attribute by name alone - no package to reference, no namespace
// it has to live in - which is why we declare our own.
//
// It does not survive a delegate hop: calling the helper through a Dictionary<Type, Action> is Action.Invoke as
// far as the analyser can see.
[AttributeUsage(AttributeTargets.Method)]
public sealed class AssertionMethodAttribute : Attribute
{
}
