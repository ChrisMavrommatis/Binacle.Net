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

	if (item.x <= Sizes.eightBitsMax && item.y <= Sizes.eightBitsMax && item.z <= Sizes.eightBitsMax) {
		return BitSize.Eight;
	}
	if (item.x <= Sizes.sixteenBitsMax && item.y <= Sizes.sixteenBitsMax && item.z <= Sizes.sixteenBitsMax) {
		return BitSize.Sixteen;
	}
	if (item.x <= Sizes.thirtyTwoBitsMax && item.y <= Sizes.thirtyTwoBitsMax && item.z <= Sizes.thirtyTwoBitsMax) {
		return BitSize.ThirtyTwo;
	}
	// The 64-bit bucket caps at maxInteger (2^53 - 1), not the full 64-bit range. JS numbers cannot hold
	// integers above that exactly, so the protocol forbids them — see vipaq/PROTOCOL.md.
	if (item.x <= Sizes.maxInteger && item.y <= Sizes.maxInteger && item.z <= Sizes.maxInteger) {
		return BitSize.SixtyFour;
	}
	// At least one axis is above maxInteger — outside ViPaq's range (PROTOCOL.md §5). Reachable in TS: a
	// float like 1e19 passes every check above. (In C# the type system stops this.) Name the offender, like
	// the negative checks above, so the message matches C#'s per-field ParamName.
	if (item.x > Sizes.maxInteger) {
		throw new Error(`'x' exceeds the max supported value (${Sizes.maxInteger})`);
	}
	if (item.y > Sizes.maxInteger) {
		throw new Error(`'y' exceeds the max supported value (${Sizes.maxInteger})`);
	}
	throw new Error(`'z' exceeds the max supported value (${Sizes.maxInteger})`);
}
