using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Abstractions.Algorithms;

public interface IPackingAlgorithm
{
	public Algorithm Algorithm { get; }
	public int Version { get; }
	OperationResult Execute(IOperationParameters parameters);
}
