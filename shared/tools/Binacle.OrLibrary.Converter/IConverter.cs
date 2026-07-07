namespace Binacle.OrLibrary.Converter;

// One dataset converted from raw OR-Library text into the tests-kernel JSON. Each implementation reads its raw
// inputs, writes its files, and logs what it wrote. Program runs every converter with no arguments; add one here
// to have it produced too.
public interface IConverter
{
	void Convert();
}
