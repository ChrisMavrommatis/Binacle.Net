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

	if (item.length <= Sizes.eightBitsMax && item.width <= Sizes.eightBitsMax && item.height <= Sizes.eightBitsMax) {
		return BitSize.Eight;
	}
	if (item.length <= Sizes.sixteenBitsMax && item.width <= Sizes.sixteenBitsMax && item.height <= Sizes.sixteenBitsMax) {
		return BitSize.Sixteen;
	}
	if (item.length <= Sizes.thirtyTwoBitsMax && item.width <= Sizes.thirtyTwoBitsMax && item.height <= Sizes.thirtyTwoBitsMax) {
		return BitSize.ThirtyTwo;
	}
	// The 64-bit bucket caps at maxInteger (2^53 - 1), not the full 64-bit range. JS numbers cannot hold
	// integers above that exactly, so the protocol forbids them — see vipaq/PROTOCOL.md.
	if (item.length <= Sizes.maxInteger && item.width <= Sizes.maxInteger && item.height <= Sizes.maxInteger) {
		return BitSize.SixtyFour;
	}
	// At least one field is above maxInteger — outside ViPaq's range (PROTOCOL.md §5). Reachable in TS: a
	// float like 1e19 passes every check above. (In C# the type system stops this.) Name the offender, like
	// the non-positive checks above, so the message matches C#'s per-field ParamName.
	if (item.length > Sizes.maxInteger) {
		throw new Error(`'length' exceeds the max supported value (${Sizes.maxInteger})`);
	}
	if (item.width > Sizes.maxInteger) {
		throw new Error(`'width' exceeds the max supported value (${Sizes.maxInteger})`);
	}
	throw new Error(`'height' exceeds the max supported value (${Sizes.maxInteger})`);
}
