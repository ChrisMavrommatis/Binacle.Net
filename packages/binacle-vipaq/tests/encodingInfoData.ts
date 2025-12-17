import {BitSize, Version} from "../src/models";

export default class EncodingInfoData {
	public version: Version;
	public binDimensionBitSize: BitSize;
	public itemDimensionsBitSize: BitSize;
	public itemCoordinatesBitSize: BitSize;
	public expectedByte: number;

	constructor(
		version: Version,
		binDimensionBitSize: BitSize,
		itemDimensionsBitSize: BitSize,
		itemCoordinatesBitSize: BitSize,
		expectedByte: number
	) {
		this.version = version;
		this.binDimensionBitSize = binDimensionBitSize;
		this.itemDimensionsBitSize = itemDimensionsBitSize;
		this.itemCoordinatesBitSize = itemCoordinatesBitSize;
		this.expectedByte = expectedByte;
	}

	static All: EncodingInfoData[] = [
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.Eight, 0b00_00_00_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.Sixteen, 0b00_00_00_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.ThirtyTwo, 0b00_00_00_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Eight, BitSize.SixtyFour, 0b00_00_00_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.Eight, 0b00_00_01_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.Sixteen, 0b00_00_01_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_00_01_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.Sixteen, BitSize.SixtyFour, 0b00_00_01_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Eight, 0b00_00_10_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_00_10_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_00_10_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_00_10_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.Eight, 0b00_00_11_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.Sixteen, 0b00_00_11_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_00_11_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Eight, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_00_11_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.Eight, 0b00_01_00_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.Sixteen, 0b00_01_00_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.ThirtyTwo, 0b00_01_00_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Eight, BitSize.SixtyFour, 0b00_01_00_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.Eight, 0b00_01_01_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.Sixteen, 0b00_01_01_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_01_01_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.Sixteen, BitSize.SixtyFour, 0b00_01_01_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Eight, 0b00_01_10_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_01_10_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_01_10_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_01_10_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Eight, 0b00_01_11_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Sixteen, 0b00_01_11_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_01_11_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.Sixteen, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_01_11_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Eight, 0b00_10_00_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Sixteen, 0b00_10_00_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.ThirtyTwo, 0b00_10_00_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Eight, BitSize.SixtyFour, 0b00_10_00_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Eight, 0b00_10_01_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Sixteen, 0b00_10_01_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_10_01_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.SixtyFour, 0b00_10_01_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Eight, 0b00_10_10_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_10_10_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_10_10_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_10_10_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Eight, 0b00_10_11_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Sixteen, 0b00_10_11_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_10_11_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_10_11_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.Eight, 0b00_11_00_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.Sixteen, 0b00_11_00_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.ThirtyTwo, 0b00_11_00_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Eight, BitSize.SixtyFour, 0b00_11_00_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Eight, 0b00_11_01_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Sixteen, 0b00_11_01_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.ThirtyTwo, 0b00_11_01_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.Sixteen, BitSize.SixtyFour, 0b00_11_01_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Eight, 0b00_11_10_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Sixteen, 0b00_11_10_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b00_11_10_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b00_11_10_11),

		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Eight, 0b00_11_11_00),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Sixteen, 0b00_11_11_01),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b00_11_11_10),
		new EncodingInfoData(Version.Uncompressed, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.SixtyFour, 0b00_11_11_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.Eight, 0b01_00_00_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.Sixteen, 0b01_00_00_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.ThirtyTwo, 0b01_00_00_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Eight, BitSize.SixtyFour, 0b01_00_00_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.Eight, 0b01_00_01_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.Sixteen, 0b01_00_01_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_00_01_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.Sixteen, BitSize.SixtyFour, 0b01_00_01_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Eight, 0b01_00_10_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_00_10_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_00_10_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_00_10_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.Eight, 0b01_00_11_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.Sixteen, 0b01_00_11_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_00_11_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Eight, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_00_11_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.Eight, 0b01_01_00_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.Sixteen, 0b01_01_00_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.ThirtyTwo, 0b01_01_00_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Eight, BitSize.SixtyFour, 0b01_01_00_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.Eight, 0b01_01_01_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.Sixteen, 0b01_01_01_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_01_01_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.Sixteen, BitSize.SixtyFour, 0b01_01_01_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Eight, 0b01_01_10_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_01_10_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_01_10_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_01_10_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Eight, 0b01_01_11_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.Sixteen, 0b01_01_11_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_01_11_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.Sixteen, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_01_11_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Eight, 0b01_10_00_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.Sixteen, 0b01_10_00_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.ThirtyTwo, 0b01_10_00_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Eight, BitSize.SixtyFour, 0b01_10_00_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Eight, 0b01_10_01_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.Sixteen, 0b01_10_01_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_10_01_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.Sixteen, BitSize.SixtyFour, 0b01_10_01_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Eight, 0b01_10_10_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_10_10_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_10_10_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_10_10_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Eight, 0b01_10_11_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.Sixteen, 0b01_10_11_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_10_11_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.ThirtyTwo, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_10_11_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.Eight, 0b01_11_00_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.Sixteen, 0b01_11_00_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.ThirtyTwo, 0b01_11_00_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Eight, BitSize.SixtyFour, 0b01_11_00_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Eight, 0b01_11_01_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.Sixteen, 0b01_11_01_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.ThirtyTwo, 0b01_11_01_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.Sixteen, BitSize.SixtyFour, 0b01_11_01_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Eight, 0b01_11_10_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.Sixteen, 0b01_11_10_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.ThirtyTwo, 0b01_11_10_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.ThirtyTwo, BitSize.SixtyFour, 0b01_11_10_11),

		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Eight, 0b01_11_11_00),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.Sixteen, 0b01_11_11_01),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.ThirtyTwo, 0b01_11_11_10),
		new EncodingInfoData(Version.CompressedGzip, BitSize.SixtyFour, BitSize.SixtyFour, BitSize.SixtyFour, 0b01_11_11_11)
	]
}

