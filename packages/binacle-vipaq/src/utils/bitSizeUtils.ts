import {BitSize} from "../models/BitSize";
import Bin from "../models/Bin";
import Sizes from "./sizes";
import Item from "../models/Item";

export function getDimensionsBitSize(item: Bin): BitSize {

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

export function getCoordinatesBitSize(item: Item): BitSize {
	if (item.x <= 0) {
		throw new Error(`'x' must be greater than 0`);
	}
	if (item.y <= 0) {
		throw new Error(`'y' must be greater than 0`);
	}
	if (item.width <= 0) {
		throw new Error(`'width' must be greater than 0`);
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


export function getByteSize(bitSize: BitSize): number {
	switch (bitSize) {
		case BitSize.Eight:
			return 1;
		case BitSize.Sixteen:
			return 2;
		case BitSize.ThirtyTwo:
			return 3;
		case BitSize.SixtyFour:
			return 4;
		default:
			throw new Error(`bitSize ${bitSize} is not supported`)
	}
}
