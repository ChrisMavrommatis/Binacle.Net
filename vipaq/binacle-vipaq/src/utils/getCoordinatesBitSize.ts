import {BitSize, Coordinates, Dimensions} from "../models";
import { Sizes } from "./sizes";

export function getCoordinatesBitSize(item: (Dimensions & Coordinates)): BitSize {
	if (item.x < 0) {
		throw new Error(`'x' must be zero or positive`);
	}
	if (item.y < 0) {
		throw new Error(`'y' must be zero or positive`);
	}
	if (item.z < 0) {
		throw new Error(`'z' must be zero or positive`);
	}

	if (item.x <= Sizes.byteMaxSize && item.y <= Sizes.byteMaxSize && item.z <= Sizes.byteMaxSize) {
		return BitSize.Eight;
	}
	if (item.x <= Sizes.uShortMaxValue && item.y <= Sizes.uShortMaxValue && item.z <= Sizes.uShortMaxValue) {
		return BitSize.Sixteen;
	}
	if (item.x <= Sizes.uIntMaxValue && item.y <= Sizes.uIntMaxValue && item.z <= Sizes.uIntMaxValue) {
		return BitSize.ThirtyTwo;
	}
	if (item.x <= Sizes.uLongMaxValue && item.y <= Sizes.uLongMaxValue && item.z <= Sizes.uLongMaxValue) {
		return BitSize.SixtyFour;
	}
	throw new Error(`The 'item' coordinates are too large`);
}
