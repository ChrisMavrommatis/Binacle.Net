// ports C#: BitSizeSelectionTests
// Kind splits the rows: dimensions run the dimensions picker, coordinates run the coordinates picker; each
// must return the expected width. The two sets together cover every bucket, pinning the pickers together.
import {getDimensionsBitSize, getCoordinatesBitSize} from "../src/utils";
import {dimensionCases, coordinateCases} from "./providers/BitSizeSelection";

describe("pickers choose the expected width", () => {
	// ports C#: Picks_Expected_Width_For_Dimensions
	test.each(dimensionCases)("dimensions: $name", ({value, expected}) => {
		expect(getDimensionsBitSize(value)).toBe(expected);
	});

	// ports C#: Picks_Expected_Width_For_Coordinates
	test.each(coordinateCases)("coordinates: $name", ({value, expected}) => {
		expect(getCoordinatesBitSize(value)).toBe(expected);
	});
});
