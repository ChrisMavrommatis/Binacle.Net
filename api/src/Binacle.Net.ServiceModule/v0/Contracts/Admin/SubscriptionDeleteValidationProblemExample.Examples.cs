using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class SubscriptionDeleteValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Id", [ErrorMessage.IdMustBeGuid] }
		};
	}
}
