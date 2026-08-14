namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// No fixture - the collection exists to keep these classes off each other. Feature.Manager is a process-wide
// static that each host build sets and then reads to pick its modules, so two builds running at once read each
// other's flags, and the symptom is a missing module rather than an error.
[CollectionDefinition(nameof(RateLimiterCollection), DisableParallelization = true)]
public sealed class RateLimiterCollection;
