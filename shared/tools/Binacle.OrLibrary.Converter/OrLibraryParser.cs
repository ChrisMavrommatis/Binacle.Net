using Binacle.Geometry;

namespace Binacle.OrLibrary.Converter;

// One packing problem read from a raw OR-Library thpack file: a container and the box types to pack into it.
internal sealed record RawProblem(int Index, int BinLength, int BinWidth, int BinHeight, IReadOnlyList<RawBoxType> BoxTypes);

// One box type: its dimensions and how many of them. Implements the read-only geometry interfaces so it can be
// handed straight to CompactNotationFormatter — no separate adapter needed.
internal sealed record RawBoxType(int Length, int Width, int Height, int Quantity)
	: IWithReadOnlyDimensions<int>, IWithReadOnlyQuantity<int>;

// Parses the raw OR-Library container-loading text for thpack1..7 (the Bischoff & Ratcliff instances).
//
// We read it line by line, the same way the file is laid out, so the code walks down it the way your eye does:
//
//     100                      first line, once: the number of problems P in this file
//     1 2502505                per problem, line 1: problem index, and the random seed used to generate it
//     587 233 220              per problem, line 2: the container, as length width height
//     3                        per problem, line 3: n, the number of box types that follow
//     1 108 0 76 0 30 1 40     then n lines, one per box type (fields explained below)
//     2 110 0 43 1 25 1 33
//     3 92 1 81 1 55 1 39
//     2 2502605                ...then the next problem begins, and so on for all P problems
//
// A box-type line has 8 fields:  type, length, flag, width, flag, height, flag, quantity
// Each dimension is followed by a 0/1 flag: 1 = that side may face up (vertical placement allowed), 0 = it may
// not. Our packer doesn't use the flags, so we read past them.
//
// (There is no per-problem "results" line — the raw text carries no packing outcome. Result is decided elsewhere;
// see BischoffSuiteConverter.)
internal static class OrLibraryParser
{
	public static IReadOnlyList<RawProblem> Parse(string text)
	{
		using var reader = new StringReader(text);

		// First line, once: how many problems this file holds.
		var problemCount = int.Parse(ReadFields(reader)[0]);
		var problems = new List<RawProblem>(problemCount);

		for (var problem = 0; problem < problemCount; problem++)
		{
			// Line 1: problem index, then the seed. We keep the index for the name; the seed we don't need.
			var headerFields = ReadFields(reader);
			var index = int.Parse(headerFields[0]);

			// Line 2: the container, as length width height.
			var binFields = ReadFields(reader);
			var binLength = int.Parse(binFields[0]);
			var binWidth = int.Parse(binFields[1]);
			var binHeight = int.Parse(binFields[2]);

			// Line 3: how many box types follow.
			var boxTypeCount = int.Parse(ReadFields(reader)[0]);

			// One line per box type: type, length, flag, width, flag, height, flag, quantity — hence the gaps
			// in the indices below, where the type number and the orientation flags are skipped over.
			var boxTypes = new List<RawBoxType>(boxTypeCount);
			for (var boxType = 0; boxType < boxTypeCount; boxType++)
			{
				var boxTypeFields = ReadFields(reader);
				boxTypes.Add(new RawBoxType(
					Length: int.Parse(boxTypeFields[1]),
					Width: int.Parse(boxTypeFields[3]),
					Height: int.Parse(boxTypeFields[5]),
					Quantity: int.Parse(boxTypeFields[7])));
			}

			problems.Add(new RawProblem(index, binLength, binWidth, binHeight, boxTypes));
		}

		return problems;
	}

	// Returns the next non-blank line split into its whitespace-separated fields. Skipping blank lines means stray
	// trailing newlines or extra spacing between problems don't throw the reader off.
	private static string[] ReadFields(StringReader reader)
	{
		string? line;
		while ((line = reader.ReadLine()) is not null)
		{
			var fields = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			if (fields.Length > 0)
			{
				return fields;
			}
		}

		throw new InvalidDataException("Unexpected end of OR-Library file - the problem count and the data disagree.");
	}
}
