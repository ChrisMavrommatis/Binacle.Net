import {EncodingInfo} from "../models";

export function encodingInfoToByte(encodingInfo: EncodingInfo): number {
	let encodingInfoByte = 0;
	encodingInfoByte |= (encodingInfo.version << 6);
	encodingInfoByte |= (encodingInfo.binDimensionsBitSize << 4);
	encodingInfoByte |= (encodingInfo.itemDimensionsBitSize << 2);
	encodingInfoByte |= encodingInfo.itemCoordinatesBitSize;
	return encodingInfoByte;
}
