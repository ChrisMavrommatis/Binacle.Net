import {Coordinates, Dimensions, Header, Item} from "../models";
import {ProtocolReader} from "../ProtocolReader";
import {ProtocolWriter} from "../ProtocolWriter";

// Ports C#: ColumnarCodec. Each field for every item before the next field — six runs, each `count` values long
// (PROTOCOL.md §3.2):
//
//   L L L ... | W W W ... | H H H ... | X X X ... | Y Y Y ... | Z Z Z ...
//
// Like magnitudes sit next to each other, which usually compresses better. Uncompressed it is exactly as long
// as row-major.
export function writeColumnar(writer: ProtocolWriter, items: (Dimensions & Coordinates)[], header: Header): void {
	const dimensionsWidth = header.itemDimensionsWidth;
	const coordinatesWidth = header.itemCoordinatesWidth;

	for (const item of items) {
		writer.writeValue(item.length, dimensionsWidth);
	}

	for (const item of items) {
		writer.writeValue(item.width, dimensionsWidth);
	}
	
	for (const item of items){
		writer.writeValue(item.height, dimensionsWidth);
	}
	
	for (const item of items){
		writer.writeValue(item.x, coordinatesWidth);
	}
	
	for (const item of items){
		writer.writeValue(item.y, coordinatesWidth);
	}
	
	for (const item of items) {
		writer.writeValue(item.z, coordinatesWidth);
	}
}

export function readColumnar(reader: ProtocolReader, items: Item[], header: Header): void {
	const dimensionsWidth = header.itemDimensionsWidth;
	const coordinatesWidth = header.itemCoordinatesWidth;

	for (const item of items) {
		item.length = reader.readValue(dimensionsWidth);
	}
	
	for (const item of items) {
		item.width = reader.readValue(dimensionsWidth);
	}
	
	for (const item of items) {
		item.height = reader.readValue(dimensionsWidth);
	}
	
	for (const item of items){
		item.x = reader.readValue(coordinatesWidth);
	}
	
	for (const item of items) {
		item.y = reader.readValue(coordinatesWidth);
	}
	
	for (const item of items) {
		item.z = reader.readValue(coordinatesWidth);
	} 
}
