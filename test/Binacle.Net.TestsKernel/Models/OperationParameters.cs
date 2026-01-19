using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Net.TestsKernel.Models;

public class OperationParameters : IOperationParameters
{
	public AlgorithmOperation Operation { get; init; }
}
