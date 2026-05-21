import {BitSize, Dimensions} from "../models";
import { Sizes } from "./sizes";

export function getDimensionsBitSize(item: Dimensions): BitSize {

	if (item.length <= 0) {
		throw new Error(`'length' must be greater than 0`);
	}
	if (item.width <= 0) {
		throw new Error(`'width' must be greater than 0`);
	}
	if (item.height <= 0) {
		throw new Error(`'height' must be greater than 0`);
	}

	if (item.length <= Sizes.byteMaxSize && item.width <= Sizes.byteMaxSize && item.height <= Sizes.byteMaxSize) {
		return BitSize.Eight;
	}
	if (item.length <= Sizes.uShortMaxValue && item.width <= Sizes.uShortMaxValue && item.height <= Sizes.uShortMaxValue) {
		return BitSize.Sixteen;
	}
	if (item.length <= Sizes.uIntMaxValue && item.width <= Sizes.uIntMaxValue && item.height <= Sizes.uIntMaxValue) {
		return BitSize.ThirtyTwo;
	}
	if (item.length <= Sizes.uLongMaxValue && item.width <= Sizes.uLongMaxValue && item.height <= Sizes.uLongMaxValue) {
		return BitSize.SixtyFour;
	}
	throw new Error(`The 'item' dimensions are too large`);
}
