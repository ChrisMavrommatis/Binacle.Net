namespace Binacle.Lib.Abstractions;

public interface IAlgorithmProcessorFactory
{
    IAlgorithmProcessor Create(int binCount, int itemCount);
}
