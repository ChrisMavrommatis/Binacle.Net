import {Coordinates, Dimensions, Width} from "../models";
import {Sizes} from "./sizes";

// Ports C#: WidthHelper.GetCoordinatesWidth. The narrowest width that holds all three coordinates. Coordinates
// may be zero (an item at the origin), unlike dimensions. Only Eight and Sixteen exist now: a value above the
// 16-bit ceiling is outside the protocol and is rejected outright. Names the offending axis so the message
// matches C#'s per-field ParamName.
export function getCoordinatesWidth(item: Dimensions & Coordinates): Width {
	if (item.x < 0) {
		throw new Error(`'x' must be zero or positive`);
	}
	if (item.y < 0) {
		throw new Error(`'y' must be zero or positive`);
	}
	if (item.z < 0) {
		throw new Error(`'z' must be zero or positive`);
	}

	if (item.x <= Sizes.eightBitsMax && item.y <= Sizes.eightBitsMax && item.z <= Sizes.eightBitsMax) {
		return Width.Eight;
	}
	if (item.x <= Sizes.maxValue && item.y <= Sizes.maxValue && item.z <= Sizes.maxValue) {
		return Width.Sixteen;
	}

	if (item.x > Sizes.maxValue) {
		throw new Error(`'x' exceeds the max supported value (${Sizes.maxValue})`);
	}
	if (item.y > Sizes.maxValue) {
		throw new Error(`'y' exceeds the max supported value (${Sizes.maxValue})`);
	}
	throw new Error(`'z' exceeds the max supported value (${Sizes.maxValue})`);
}
