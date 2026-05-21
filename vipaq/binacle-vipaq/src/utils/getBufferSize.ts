import {EncodingInfo} from "../models";

import {getByteSize} from "./getByteSize";

export function getBufferSize(encodingInfo: EncodingInfo, itemsCount: number): number{

	// [Version]
	// [2 Bits]

	// [Bin Dimensions Bit Size]
	// [2 Bits]

	// [Item Dimensions Bit Size]
	// [2 Bits]

	// [Item Coordinates Bit Size]
	// [2 Bits]
	const headerSize =
		1 // Encoding Byte
		+ 2; // No Of Items

	const binBufferSize = 3 * getByteSize(encodingInfo.binDimensionsBitSize);

	const itemDimensionsBufferSize = 3 * getByteSize(encodingInfo.itemDimensionsBitSize);
	const itemCoordinatesBufferSize = 3 * getByteSize(encodingInfo.itemCoordinatesBitSize)

	const itemsBufferSize = (itemsCount * itemDimensionsBufferSize) + (itemsCount * itemCoordinatesBufferSize);

	return headerSize + binBufferSize + itemsBufferSize;
}
