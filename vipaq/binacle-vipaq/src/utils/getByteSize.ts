import {BitSize} from "../models";

export function getByteSize(bitSize: BitSize): number {
	switch (bitSize) {
		case BitSize.Eight:
			return 1;
		case BitSize.Sixteen:
			return 2;
		case BitSize.ThirtyTwo:
			return 4;
		case BitSize.SixtyFour:
			return 8;
		default:
			throw new Error(`bitSize ${bitSize} is not supported`)
	}
}
