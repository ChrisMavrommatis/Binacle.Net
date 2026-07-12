import {Coordinates, Dimensions, Header, Item} from "../models";
import {ProtocolReader} from "../ProtocolReader";
import {ProtocolWriter} from "../ProtocolWriter";

// Ports C#: RowMajorCodec. Each item whole, then the next item (PROTOCOL.md §3.1):
//
//   L W H X Y Z | L W H X Y Z | ...
//
// Easier to read in a hex dump than columnar, and exactly the same length uncompressed.
export function writeRowMajor(writer: ProtocolWriter, items: (Dimensions & Coordinates)[], header: Header): void {
	const dimensionsWidth = header.itemDimensionsWidth;
	const coordinatesWidth = header.itemCoordinatesWidth;

	for (const item of items) {
		writer.writeValue(item.length, dimensionsWidth);
		writer.writeValue(item.width, dimensionsWidth);
		writer.writeValue(item.height, dimensionsWidth);
		writer.writeValue(item.x, coordinatesWidth);
		writer.writeValue(item.y, coordinatesWidth);
		writer.writeValue(item.z, coordinatesWidth);
	}
}

export function readRowMajor(reader: ProtocolReader, items: Item[], header: Header): void {
	const dimensionsWidth = header.itemDimensionsWidth;
	const coordinatesWidth = header.itemCoordinatesWidth;

	for (const item of items) {
		item.length = reader.readValue(dimensionsWidth);
		item.width = reader.readValue(dimensionsWidth);
		item.height = reader.readValue(dimensionsWidth);
		item.x = reader.readValue(coordinatesWidth);
		item.y = reader.readValue(coordinatesWidth);
		item.z = reader.readValue(coordinatesWidth);
	}
}
