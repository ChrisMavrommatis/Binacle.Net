import {Width} from "../models";

// Ports C#: WidthHelper.ByteCount. Bytes on the wire for one value of a width. A reserved width never reaches
// the wire, so asking for its byte count is a bug or a malformed blob - throw rather than guess.
export function widthByteCount(width: Width): number {
	switch (width) {
		case Width.Eight:
			return 1;
		case Width.Sixteen:
			return 2;
		default:
			throw new Error(`width ${width} is reserved and has no byte count`);
	}
}
