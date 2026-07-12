import {Header, Version} from "../models";
import {widthByteCount} from "./widthByteCount";

// Ports C#: Header.ToBytes. Packs the header into its two wire bytes (PROTOCOL.md §2.1, §2.2). A header a caller
// built by hand can still be unwritable — a reserved version or a reserved width must never reach the wire — so
// those are checked here (widthByteCount throws on a reserved width). Both are encode-side caller errors, not
// malformed input.
export function headerToBytes(header: Header): Uint8Array<ArrayBuffer> {
	if (header.version !== Version.Version1) {
		throw new Error(`This implementation writes only version ${Version.Version1}`);
	}
	widthByteCount(header.binDimensionsWidth);
	widthByteCount(header.itemDimensionsWidth);
	widthByteCount(header.itemCoordinatesWidth);

	const bytes = new Uint8Array(Header.byteCount);
	bytes[0] =
		(header.version << 6) |
		((header.compressed ? 1 : 0) << 5) |
		(header.layout << 4);
	bytes[1] =
		(header.binDimensionsWidth << 6) |
		(header.itemDimensionsWidth << 4) |
		(header.itemCoordinatesWidth << 2);
	return bytes;
}
