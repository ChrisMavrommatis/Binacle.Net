namespace Binacle.ViPaq.Generators;

// One committed shared vector file. Each implementation reads its inputs, writes its file, and logs what it
// wrote. Program runs every generator with no arguments; add one here to have it regenerated too.
public interface IVectorGenerator
{
	void Generate();
}
