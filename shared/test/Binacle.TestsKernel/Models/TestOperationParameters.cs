using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.TestsKernel.Models;

public class TestOperationParameters : IOperationParameters
{
	public AlgorithmOperation Operation { get; init; }
}
