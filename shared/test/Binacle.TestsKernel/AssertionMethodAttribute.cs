namespace Binacle.TestsKernel;

// Marks a helper that does the asserting on a test's behalf.
//
// Sonar's S2699 ("tests should include assertions") only walks the test method body, so a test that asserts
// one call away - through a fixture helper, say - reads as assertion-free and gets flagged. The analyser
// matches this attribute by name alone: there is no package to reference and no namespace it has to live in,
// which is why we declare our own rather than take a dependency. Annotating the helper tells the rule where
// the assertion actually happens, which beats switching the rule off for test code and leaves it live to
// catch a test that genuinely checks nothing.
//
// It does not survive a delegate hop. A test that calls the helper through a Dictionary<Type, Action> is
// invoking Action.Invoke as far as the analyser can see, and no attribute on the far side is visible to it.
[AttributeUsage(AttributeTargets.Method)]
public sealed class AssertionMethodAttribute : Attribute
{
}
