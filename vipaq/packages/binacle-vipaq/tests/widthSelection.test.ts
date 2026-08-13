// ports C#: WidthSelectionTests
// Kind splits the rows: dimensions run the dimensions picker, coordinates the coordinates picker. The two sets
// together cover every bucket.
import {getDimensionsWidth, getCoordinatesWidth} from "../src/utils";
import {dimensionCases, coordinateCases} from "./providers/WidthSelection";

describe("pickers choose the expected width", () => {
	// ports C#: Picks_Expected_Width_For_Dimensions
	test.each(dimensionCases)("dimensions: $name", ({value, expected}) => {
		expect(getDimensionsWidth(value)).toBe(expected);
	});

	// ports C#: Picks_Expected_Width_For_Coordinates
	test.each(coordinateCases)("coordinates: $name", ({value, expected}) => {
		expect(getCoordinatesWidth(value)).toBe(expected);
	});
});
