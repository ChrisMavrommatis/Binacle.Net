namespace Binacle.Net.v3;

// OpenAPI property description text for the v3 contracts, referenced from [Description]. Self-contained per
// version so v3's frozen wording never shifts when v4 changes. Wording follows the docs nomenclature: bins and
// items, presets, fitting vs packing, the FFD/WFD/BFD heuristics, and the ViPaq protocol.
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
	public const string Algorithm = "The heuristic algorithm to use: FFD, WFD or BFD.";

	public const string PackedItems = "Items placed in the bin, each with its placement.";
	public const string UnpackedItems = "Items that could not be placed in the bin.";
	public const string FittedItems = "Items that fit in the bin.";
	public const string UnfittedItems = "Items that do not fit in the bin.";

	public const string PackedItemsVolumePercentage = "Percentage of the items' volume that was packed (0-100).";
	public const string PackedBinVolumePercentage = "Percentage of the bin's volume filled by the packed items (0-100).";
	public const string FittedItemsVolumePercentage = "Percentage of the items' volume that fits (0-100).";
	public const string FittedBinVolumePercentage = "Percentage of the bin's volume the fitting items occupy (0-100).";

	public const string Result = "The result of the operation.";
	public const string Data = "The response payload.";

	public const string PresetParam = "Name of the preset to use.";
}
