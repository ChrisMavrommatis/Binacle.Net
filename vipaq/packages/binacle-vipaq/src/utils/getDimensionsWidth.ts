import {Dimensions, Width} from "../models";
import {Sizes} from "./sizes";

// Ports C#: WidthHelper.GetDimensionsWidth. The narrowest width that holds all three dimensions. Dimensions
// must be positive (a zero-sized box is not a box). Only Eight and Sixteen exist now: a value above the 16-bit
// ceiling is outside the protocol and is rejected outright — there is no wider width to grow into, and no
// saturation. Reachable in TS (a float like 1e19 is a valid number); in C# the type system stops most of it.
// Names the offending field so the message matches C#'s per-field ParamName.
export function getDimensionsWidth(item: Dimensions): Width {
	if (item.length <= 0) {
		throw new Error(`'length' must be greater than 0`);
	}
	if (item.width <= 0) {
		throw new Error(`'width' must be greater than 0`);
	}
	if (item.height <= 0) {
		throw new Error(`'height' must be greater than 0`);
	}

	if (item.length <= Sizes.eightBitsMax && item.width <= Sizes.eightBitsMax && item.height <= Sizes.eightBitsMax) {
		return Width.Eight;
	}
	if (item.length <= Sizes.maxValue && item.width <= Sizes.maxValue && item.height <= Sizes.maxValue) {
		return Width.Sixteen;
	}

	if (item.length > Sizes.maxValue) {
		throw new Error(`'length' exceeds the max supported value (${Sizes.maxValue})`);
	}
	if (item.width > Sizes.maxValue) {
		throw new Error(`'width' exceeds the max supported value (${Sizes.maxValue})`);
	}
	throw new Error(`'height' exceeds the max supported value (${Sizes.maxValue})`);
}
