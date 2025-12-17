import {BitSize, Coordinates, Dimensions, EncodingInfo, Version} from "../models";
import {Sizes} from "./sizes";
import {getDimensionsBitSize} from "./getDimensionsBitSize";
import {getCoordinatesBitSize} from "./getCoordinatesBitSize";

export function createEncodingInfo(bin: Dimensions, items: (Dimensions & Coordinates)[]): EncodingInfo {
	if (items.length > Sizes.uShortMaxValue) {
		throw new Error(`Items cannot be more than ${Sizes.uShortMaxValue}`);
	}

	const binDimensionsBitSize = getDimensionsBitSize(bin);
	let itemDimensionsBitSize = BitSize.Eight;
	let itemCoordinatesBitSize = BitSize.Eight;
	for(let item of items) {
		const localItemDimensionsBitSize = getDimensionsBitSize(item);
		if(localItemDimensionsBitSize > itemDimensionsBitSize){
			itemDimensionsBitSize = localItemDimensionsBitSize;
		}
		const localItemCoordinatesBitSize = getCoordinatesBitSize(item);
		if(localItemCoordinatesBitSize > itemCoordinatesBitSize){
			itemCoordinatesBitSize = localItemCoordinatesBitSize;
		}
	}
	return new EncodingInfo(
		Version.Uncompressed,
		binDimensionsBitSize,
		itemDimensionsBitSize,
		itemCoordinatesBitSize,
	);
}
