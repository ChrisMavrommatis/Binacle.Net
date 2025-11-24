import EncodingInfoData from "../encodingInfoData";
import EncodingInfo from "../../src/models/EncodingInfo";
import {encodingInfoFromByte, encodingInfoToByte} from "../../src/utils/encodingUtils";
import {faker} from '@faker-js/faker';

faker.seed(605080);



describe('encodingInfoUtils', () => {
	test.each(EncodingInfoData.All)("encodingInfoToByte Returns Correct Byte",
		(encodingInfoData) => {

			const encodingInfo = new EncodingInfo(
				encodingInfoData.version,
				encodingInfoData.binDimensionBitSize,
				encodingInfoData.itemDimensionsBitSize,
				encodingInfoData.itemCoordinatesBitSize
			);

			const actualByte = encodingInfoToByte(encodingInfo);

			expect(actualByte).toBe(encodingInfoData.expectedByte);
		}
	);

	test.each(EncodingInfoData.All)("encodingInfoFromByte Returns Correct EncodingInfo",
		(encodingInfoData) => {

			const encodingInfo = encodingInfoFromByte(encodingInfoData.expectedByte);

			expect(encodingInfo.version).toBe(encodingInfoData.version);
			expect(encodingInfo.binDimensionsBitSize).toBe(encodingInfoData.binDimensionBitSize);
			expect(encodingInfo.itemDimensionsBitSize).toBe(encodingInfoData.itemDimensionsBitSize);
			expect(encodingInfo.itemCoordinatesBitSize).toBe(encodingInfoData.itemCoordinatesBitSize);
		}
	);


});


