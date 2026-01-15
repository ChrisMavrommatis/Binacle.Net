using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Models;

internal class OperationParameters : ILogConvertible, IOperationParameters
{
	public required Algorithm Algorithm { get; init; }
	public required	AlgorithmOperation Operation { get; init; }

	
	public object ConvertToLogObject()
	{
		// Algorithm Is Always added
		var paramsCount = 2;

		var parameters = new string[paramsCount];

		parameters[--paramsCount] = this.Algorithm.ToFastString();
		parameters[--paramsCount] = this.Operation.ToFastString();
		
		return parameters;
	}
	
}
