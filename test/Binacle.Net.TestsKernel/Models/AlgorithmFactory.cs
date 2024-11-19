namespace Binacle.Net.TestsKernel.Models;

public delegate TAlgorithm AlgorithmFactory<out TAlgorithm>(TestBin bin, List<TestItem> items);
