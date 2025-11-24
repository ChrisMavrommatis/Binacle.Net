import EncodingInfo from "../models/EncodingInfo";
import Item from "../models/Item";
import Bin from "../models/Bin";
import Sizes from "./sizes";
import {getCoordinatesBitSize, getDimensionsBitSize} from "./bitSizeUtils";
import {BitSize} from "../models/BitSize";
import {Version} from "../models/Version";

export default function createEncodingInfo(bin: Bin, items: Item[]): EncodingInfo {
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
