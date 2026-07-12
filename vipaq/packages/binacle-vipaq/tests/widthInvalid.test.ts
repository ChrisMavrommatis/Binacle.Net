// ports C#: WidthInvalidTests
//
// C# asserts ArgumentOutOfRangeException.ParamName == Field (PascalCase). TS pickers throw a plain Error whose
// MESSAGE contains the field LOWERCASED (e.g. `'length'`, `'x'`), so we assert on the lowercased field. Kind
// splits the rows: dimensions run the dimensions picker, coordinates run the coordinates picker.
//
// There is no saturation any more: the old 32/64-bit tiers are gone, so a value above the 16-bit ceiling (65535)
// is rejected outright rather than picking a wider width. Those over-ceiling cases are the "exceeds max" rows.
import {getDimensionsWidth, getCoordinatesWidth} from "../src/utils";
import {dimensionCases, coordinateCases} from "./providers/WidthInvalid";

describe("pickers reject invalid values", () => {
	// ports C#: Dimensions_Picker_Throws_For_Offending_Field
	test.each(dimensionCases)("dimensions: $name", ({value, field}) => {
		expect(() => getDimensionsWidth(value)).toThrow(`'${field.toLowerCase()}'`);
	});

	// ports C#: Coordinates_Picker_Throws_For_Offending_Field
	test.each(coordinateCases)("coordinates: $name", ({value, field}) => {
		expect(() => getCoordinatesWidth(value)).toThrow(`'${field.toLowerCase()}'`);
	});
});
