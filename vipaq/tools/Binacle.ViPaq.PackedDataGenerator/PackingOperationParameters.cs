using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.ViPaq.PackedDataGenerator;

// The packer's Execute takes an IOperationParameters. There is no concrete one in the lib, and offline we only
// ever do a Packing run with no logging DI — so this is the whole implementation.
internal sealed class PackingOperationParameters : IOperationParameters
{
	public AlgorithmOperation Operation => AlgorithmOperation.Packing;
}
