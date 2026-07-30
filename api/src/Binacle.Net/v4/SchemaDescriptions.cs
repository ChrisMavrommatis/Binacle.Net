namespace Binacle.Net.v4;

// OpenAPI property description text for the v4 contracts, referenced from [Description]. Self-contained per
// version so v4 can evolve its wording without touching v3. Wording follows the docs nomenclature: bins and
// items, presets, fitting vs packing, the FFD/WFD/BFD heuristics (plus Best), and the ViPaq protocol.
internal static class SchemaDescriptions
{
	public const string Id =
		"Caller-supplied identifier, echoed back on the matching result so requests and responses can be correlated.";
	public const string Length = "Length of the bin or item, in the same unit as the other dimensions.";
	public const string Width = "Width of the bin or item, in the same unit as the other dimensions.";
	public const string Height = "Height of the bin or item, in the same unit as the other dimensions.";
	public const string Quantity = "Number of identical items of this type.";
	public const string CoordinateX = "Placement of the item along the length axis, measured from the bin origin.";
	public const string CoordinateY = "Placement of the item along the width axis, measured from the bin origin.";
	public const string CoordinateZ = "Placement of the item along the height axis, measured from the bin origin.";

	public const string IncludeViPaqData = "When true, the packing result includes its ViPaq data token.";
	public const string ViPaqData =
		"The packing result encoded as a ViPaq protocol token (Base64). Present only when requested.";

	public const string Items = "The items to fit or pack.";
	public const string Bins = "The custom bins to consider.";
	public const string Bin = "The bin these results are for.";
	public const string Parameters = "Options controlling the operation, such as the algorithm to use.";
	// Best does not run the same set everywhere: the single-bin routes run all three heuristics, the multi-bin
	// routes run FFD and BFD only. Spell that out - an integrator sizing a request budget reads this line.
	public const string Algorithm =
		"The heuristic algorithm to use: FFD, WFD, BFD, or Best to run more than one and keep the best result. "
		+ "Best runs all three on fit/bin and pack/bin, and FFD plus BFD on every other route.";
	public const string AlgorithmUsed =
		"The algorithm actually used for this result. When Best is requested, this is the heuristic that won.";

	public const string PackedItems = "Items placed in the bin, each with its placement.";
	public const string UnpackedItems = "Items that could not be placed in the bin.";

	public const string PackedItemsVolumePercentage = "Percentage of the items' volume that was packed (0-100).";
	public const string PackedBinVolumePercentage = "Percentage of the bin's volume filled by the packed items (0-100).";

	public const string Status = "The outcome of the operation.";
	public const string EarlyExitReason = "Why a fit check stopped early, or None if it ran to completion.";

	public const string Results = "One result per bin considered.";
	public const string Presets = "The presets configured on the server.";
	public const string PresetName = "The preset's name.";

	public const string PresetParam = "Name of the preset to use.";
	public const string BinParam = "Identifier of the bin within the preset.";
}
