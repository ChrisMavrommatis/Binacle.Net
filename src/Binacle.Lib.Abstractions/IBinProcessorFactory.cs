namespace Binacle.Lib.Abstractions;

public interface IBinProcessorFactory
{
    IBinProcessor Create(int binCount, int itemCount);
}
