import {Header} from "../models";
import {widthByteCount} from "./widthByteCount";

// Ports C#: Header.GetBodyLength. The exact length of the body — everything after the two header bytes: the
// uint16 item count, then the bin dimensions, then the items. Fixed once the header and the item count are
// known, because no field is variable-length. The writer needs this up front to size its buffer, and decode
// uses the same number as its whole structural check (a body of exactly this length cannot be truncated and
// cannot have trailing bytes).
export function getBodyLength(header: Header, itemCount: number): number {
	const bytesPerItem = 3 * (
		widthByteCount(header.itemDimensionsWidth) +
		widthByteCount(header.itemCoordinatesWidth)
	);

	const countBytes = 2; // uint16 item count (PROTOCOL.md §3)
	const binBytes = 3 * widthByteCount(header.binDimensionsWidth);

	return countBytes + binBytes + (itemCount * bytesPerItem);
}
