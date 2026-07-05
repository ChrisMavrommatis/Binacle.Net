// ports C#: BitSizeInvalidTests
//
// C# asserts ArgumentOutOfRangeException.ParamName == Field (PascalCase). TS pickers throw a plain Error
// whose MESSAGE contains the field LOWERCASED (e.g. `'length'`, `'x'`), so we assert on the lowercased
// field. Kind splits the rows: dimensions run the dimensions picker, coordinates run the coordinates picker.
import {getDimensionsBitSize, getCoordinatesBitSize} from "../src/utils";
import {dimensionCases, coordinateCases} from "./providers/BitSizeInvalid";

describe("pickers reject invalid values", () => {
	// ports C#: Dimensions_Picker_Throws_For_Offending_Field
	test.each(dimensionCases)("dimensions: $name", ({value, field}) => {
		expect(() => getDimensionsBitSize(value)).toThrow(`'${field.toLowerCase()}'`);
	});

	// ports C#: Coordinates_Picker_Throws_For_Offending_Field
	test.each(coordinateCases)("coordinates: $name", ({value, field}) => {
		expect(() => getCoordinatesBitSize(value)).toThrow(`'${field.toLowerCase()}'`);
	});
});
