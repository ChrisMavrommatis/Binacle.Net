import {BitSize} from '../models/BitSize';
import {EncodingInfo} from "../models/EncodingInfo";
import {Version} from "../models/Version";


export class EncodingInfoHelper {
	private static bitSizes: Record<string, BitSize> = {
		// For byte, sbyte, short, ushort, int, uint, long, ulong, use 'number'
		'byte': BitSize.Eight,
		'sbyte': BitSize.Eight,
		'short': BitSize.Sixteen,
		'ushort': BitSize.Sixteen,
		'int': BitSize.ThirtyTwo,
		'uint': BitSize.ThirtyTwo,
		'long': BitSize.SixtyFour,
		'ulong': BitSize.SixtyFour,
	};

	public static toByte(encodingInfo: EncodingInfo): number {
		let encodingInfoByte = 0;
		encodingInfoByte |= (encodingInfo.version << 6);
		encodingInfoByte |= (encodingInfo.binDimensionsBitSize << 4);
		encodingInfoByte |= (encodingInfo.itemDimensionsBitSize << 2);
		encodingInfoByte |= encodingInfo.itemCoordinatesBitSize;
		return encodingInfoByte;
	}

	public static fromByte(firstByte: number): EncodingInfo {
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
}
