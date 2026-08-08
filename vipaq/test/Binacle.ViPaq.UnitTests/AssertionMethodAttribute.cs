namespace Binacle.ViPaq.UnitTests;

// Marks a helper that does the asserting on a test's behalf. See the copy in Binacle.TestsKernel for the
// full story - this project does not reference that one, and the analyser matches the attribute by name
// alone, so declaring it again here is the whole cost of having it.
[AttributeUsage(AttributeTargets.Method)]
public sealed class AssertionMethodAttribute : Attribute
{
}
