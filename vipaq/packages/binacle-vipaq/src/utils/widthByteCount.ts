import {Width} from "../models";

// Ports C#: WidthHelper.ByteCount. How many bytes one value of a width takes on the wire. A reserved width
// never reaches the wire, so asking for its byte count is a bug (encode side) or a malformed blob (decode
// side) — either way, throw rather than guess.
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
