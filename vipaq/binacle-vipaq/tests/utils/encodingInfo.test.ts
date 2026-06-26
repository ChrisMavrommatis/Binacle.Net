// mirrors src/utils/encodingInfoToByte.ts + encodingInfoFromByte.ts (inverse pair -> one file)
// 128-row truth table: every Version x Bin x ItemDims x ItemCoords combo packs to its byte and back.
// Data lives in providers/encodingInfoCases.ts. ports C#: EncodingInfoByteTests.
import EncodingInfoData from "../providers/encodingInfoCases";
import {EncodingInfo} from "../../src/models";
import {encodingInfoToByte, encodingInfoFromByte} from "../../src/utils";

function toInfo(data: EncodingInfoData): EncodingInfo {
	return new EncodingInfo(data.version, data.binDimensionBitSize, data.itemDimensionsBitSize, data.itemCoordinatesBitSize);
}

describe("encodingInfo packing", () => {
	// ports C#: ToByte_Returns_Correct_Byte
	test.each(EncodingInfoData.All)("encodingInfoToByte packs to byte $expectedByte", (data) => {
		expect(encodingInfoToByte(toInfo(data))).toBe(data.expectedByte);
	});

	// ports C#: FromByte_Returns_Correct_EncodingInfo
	test.each(EncodingInfoData.All)("encodingInfoFromByte unpacks byte $expectedByte", (data) => {
		const info = encodingInfoFromByte(data.expectedByte);
		expect(info.version).toBe(data.version);
		expect(info.binDimensionsBitSize).toBe(data.binDimensionBitSize);
		expect(info.itemDimensionsBitSize).toBe(data.itemDimensionsBitSize);
		expect(info.itemCoordinatesBitSize).toBe(data.itemCoordinatesBitSize);
	});

	// ports C#: ToByte_Then_FromByte_Returns_Original
	test.each(EncodingInfoData.All)("round trips byte $expectedByte", (data) => {
		const restored = encodingInfoFromByte(encodingInfoToByte(toInfo(data)));
		expect(restored.version).toBe(data.version);
		expect(restored.binDimensionsBitSize).toBe(data.binDimensionBitSize);
		expect(restored.itemDimensionsBitSize).toBe(data.itemDimensionsBitSize);
		expect(restored.itemCoordinatesBitSize).toBe(data.itemCoordinatesBitSize);
	});
});
