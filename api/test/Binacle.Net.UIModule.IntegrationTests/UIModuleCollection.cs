namespace Binacle.Net.UIModule.IntegrationTests;

// Feature.Manager is process-global static state, set by Program.cs while a host builds. Two hosts that
// disagree about UI_MODULE cannot be alive at once - the second to boot answers for both - so every class
// here runs one at a time. The ServiceModule's rate limiting suite serialises for the same reason.
[CollectionDefinition(nameof(UIModuleCollection), DisableParallelization = true)]
public class UIModuleCollection;
