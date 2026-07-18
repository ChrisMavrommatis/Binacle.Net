namespace Binacle.Net.Kernel.OpenApi.Models;

// Carries the operationId so it can be set independently of the endpoint name. Endpoint names must be unique
// across the whole app (v3 and v4 both have e.g. "listPresets"); operationIds only need to be unique within one
// document, so keeping them off the endpoint name lets each version keep the clean, unqualified id.
internal sealed class OperationIdMetadata
{
	internal OperationIdMetadata(string operationId)
	{
		this.OperationId = operationId;
	}

	public string OperationId { get; }
}
