import {Header} from "../models";
import {widthByteCount} from "./widthByteCount";

// Ports C#: Header.GetBodyLength. Everything after the two header bytes: the uint16 item count, the bin
// dimensions, then the items. Nothing is variable-length, so this one number sizes the writer's buffer and is
// decode's whole structural check - an exact-length body cannot be truncated and cannot have trailing bytes.
export function getBodyLength(header: Header, itemCount: number): number {
	const bytesPerItem = 3 * (
		widthByteCount(header.itemDimensionsWidth) +
		widthByteCount(header.itemCoordinatesWidth)
	);

	const countBytes = 2; // uint16 item count (PROTOCOL.md §3)
	const binBytes = 3 * widthByteCount(header.binDimensionsWidth);

	return countBytes + binBytes + (itemCount * bytesPerItem);
}
