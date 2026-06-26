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
	// The 64-bit bucket caps at maxInteger (2^53 - 1), not the full 64-bit range. JS numbers cannot hold
	// integers above that exactly, so the protocol forbids them — see vipaq/PROTOCOL.md.
	if (item.length <= Sizes.maxInteger && item.width <= Sizes.maxInteger && item.height <= Sizes.maxInteger) {
		return BitSize.SixtyFour;
	}
	// Reachable in TS: a float like 1e19 passes every check above. (In C# the type system stops this.)
	throw new Error(`The 'item' dimensions exceed the max supported value (${Sizes.maxInteger})`);
}
