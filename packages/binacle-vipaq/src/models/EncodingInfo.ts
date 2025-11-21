import { Version } from "./Version";
import { BitSize } from "./BitSize";

export class EncodingInfo {
	// 2 Bits
	public version: Version;
	// 2 bits
	public binDimensionsBitSize: BitSize;
	// 2 bits
	public itemDimensionsBitSize: BitSize;
	// 2 bits
	public itemCoordinatesBitSize: BitSize;

	constructor(
		version: Version,
		binDimensionsBitSize: BitSize,
		itemDimensionsBitSize: BitSize,
		itemCoordinatesBitSize: BitSize
	) {
		this.version = version;
		this.binDimensionsBitSize = binDimensionsBitSize;
		this.itemDimensionsBitSize = itemDimensionsBitSize;
		this.itemCoordinatesBitSize = itemCoordinatesBitSize;
	}

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

	public toByte(): number {
		let encodingInfoByte = 0;
		encodingInfoByte |= (this.version << 6);
		encodingInfoByte |= (this.binDimensionsBitSize << 4);
		encodingInfoByte |= (this.itemDimensionsBitSize << 2);
		encodingInfoByte |= this.itemCoordinatesBitSize;
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
