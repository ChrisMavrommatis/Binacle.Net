// mirrors src/utils/encodingInfoToByte.ts + encodingInfoFromByte.ts (inverse pair -> one file)
// ports C#: EncodingInfoByteTests. All 256 combos: every EncodingInfo string packs to its byte and back.
import {encodingInfoCases} from "../providers/EncodingInfoCases";
import {encodingInfoToByte, encodingInfoFromByte} from "../../src/utils";

describe("encodingInfo packing", () => {
	// ports C#: ToByte_Returns_Correct_Byte
	test.each(encodingInfoCases)("$encodingInfo packs to its byte", ({info, byte}) => {
		expect(encodingInfoToByte(info)).toBe(byte);
	});

	// ports C#: FromByte_Returns_Correct_EncodingInfo
	test.each(encodingInfoCases)("$encodingInfo unpacks from its byte", ({info, byte}) => {
		const decoded = encodingInfoFromByte(byte);
		expect(decoded.version).toBe(info.version);
		expect(decoded.binDimensionsBitSize).toBe(info.binDimensionsBitSize);
		expect(decoded.itemDimensionsBitSize).toBe(info.itemDimensionsBitSize);
		expect(decoded.itemCoordinatesBitSize).toBe(info.itemCoordinatesBitSize);
	});

	// ports C#: ToByte_Then_FromByte_Returns_Original
	test.each(encodingInfoCases)("$encodingInfo round trips", ({info}) => {
		const restored = encodingInfoFromByte(encodingInfoToByte(info));
		expect(restored.version).toBe(info.version);
		expect(restored.binDimensionsBitSize).toBe(info.binDimensionsBitSize);
		expect(restored.itemDimensionsBitSize).toBe(info.itemDimensionsBitSize);
		expect(restored.itemCoordinatesBitSize).toBe(info.itemCoordinatesBitSize);
	});
});
