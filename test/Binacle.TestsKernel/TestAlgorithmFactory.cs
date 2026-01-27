using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel;

public delegate TAlgorithm TestAlgorithmFactory<out TAlgorithm>(TestBin bin, List<TestItem> items);
