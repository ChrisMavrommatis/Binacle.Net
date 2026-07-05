import {EncodingInfo} from "../models";

export function encodingInfoFromByte(firstByte: number): EncodingInfo {
	const version = (firstByte & 0b11000000) >> 6;
	const binDimensionsBitSize = (firstByte & 0b00110000) >> 4;
	const itemDimensionsBitSize = (firstByte & 0b00001100) >> 2;
	const itemCoordinatesBitSize = firstByte & 0b00000011;

	return new EncodingInfo(
		version,
		binDimensionsBitSize,
		itemDimensionsBitSize,
		itemCoordinatesBitSize
	);
}
