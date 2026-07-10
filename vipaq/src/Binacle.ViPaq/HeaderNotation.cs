namespace Binacle.ViPaq;

// STUB — not implemented. The text form of a `Header`, used by the test vectors and the interop generators so
// a vector can name the exact header its bytes were produced under (PROTOCOL.md §6.1 requires this: byte
// comparison is only meaningful once the header is pinned).
//
// It stays in the library because it is wire-specific — it names `Header`, `Width`, `Layout` and `Version`,
// which the shared `Binacle.CompactNotation` leaf cannot hold. The geometry notation (dimensions, coordinates,
// items — `"10x10x10 (0,0,0)"`) is not here and never was; it lives in that leaf.
//
// ## The grammar is not decided
//
// The old notation was `Version_Bin_ItemDim_ItemCoord`, e.g. `"Uncompressed_8_8_8"`. It does not survive the
// rebuild: its `Version` word conflated the version with the compression flag (`"Compressed"` meant
// `CompressedGzip`), and those are now two separate header fields. The new header also adds `Layout`, which the
// old grammar has no place for, and drops the 32- and 64-bit widths.
//
// So a new grammar has to carry, at minimum: `Compressed`, `Layout`, and the three widths. Open questions:
//
//   - Does `Version` appear at all? It is always `Version1` today, so it is noise — but omitting it means the
//     notation cannot name a blob from a future version.
//   - How are the flags spelled? Positional (`"raw_row_8_8_8"`) reads short; named (`"compressed=1,layout=row,..."`)
//     survives a new field being added without silently shifting every position.
//   - The existing vectors under `vipaq/test-vectors/` are all written in the old grammar, so whatever is chosen
//     here, they get regenerated (see `.agents/plans/vipaq/architecture.md`, "Open").
//
// Settle this when the vectors are regenerated, with the vector files in front of you. Do not guess it here.
internal static class HeaderNotation
{
	public static Header Parse(string compact)
	{
		throw new NotImplementedException("HeaderNotation.Parse: the header text grammar is not decided yet.");
	}

	public static string Format(Header header)
	{
		throw new NotImplementedException("HeaderNotation.Format: the header text grammar is not decided yet.");
	}
}
