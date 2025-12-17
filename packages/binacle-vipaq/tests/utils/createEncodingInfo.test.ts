import {createEncodingInfo, Sizes} from '../../src/utils';
import {BitSize} from "../../src/models";
import {createFakeBin, createFakeItems} from "../utils";
import {faker} from '@faker-js/faker';


faker.seed(605080);

const testData = [
	[1, Sizes.byteMaxSize, BitSize.Eight],
	[Sizes.byteMaxSize + 1, Sizes.uShortMaxValue, BitSize.Sixteen],
	[Sizes.uShortMaxValue + 1, Sizes.uIntMaxValue, BitSize.ThirtyTwo],
	[Sizes.uIntMaxValue + 1, Sizes.uLongMaxValue, BitSize.SixtyFour]
];

describe('createEncodingInfo', () => {
	test.each(testData)("createEncodingInfo Creates EncodingInfo With Correct Bit Sizes",
		(minValue, maxValue, expectedBitSize) => {

			const bin = createFakeBin(minValue, maxValue);
			const items = createFakeItems(minValue, maxValue, minValue, maxValue, 3);

			const encodingInfo = createEncodingInfo(bin, items);

			expect(encodingInfo.binDimensionsBitSize).toBe(expectedBitSize);
			expect(encodingInfo.itemDimensionsBitSize).toBe(expectedBitSize)
			expect(encodingInfo.itemCoordinatesBitSize).toBe(expectedBitSize)
		}
	);

	test.each(testData)("CreateEncodingInfo Creates EncodingInfo With Correct BinDimensionsBitSize",
		(minValue, maxValue, expectedBitSize) => {

			const bin = createFakeBin(minValue, maxValue);
			const items = createFakeItems(0, Sizes.byteMaxSize, 0, Sizes.byteMaxSize,3);

			const encodingInfo = createEncodingInfo(bin, items);

			expect(encodingInfo.binDimensionsBitSize).toBe(expectedBitSize);
			expect(encodingInfo.itemDimensionsBitSize).toBe(BitSize.Eight)
			expect(encodingInfo.itemCoordinatesBitSize).toBe(BitSize.Eight)
		}
	);

	test.each(testData)("CreateEncodingInfo Creates EncodingInfo With Correct ItemDimensionsBitSize",
		(minValue, maxValue, expectedBitSize) => {

			const bin = createFakeBin(0, Sizes.byteMaxSize);
			const items = createFakeItems(minValue, maxValue, 0, Sizes.byteMaxSize,3);

			const encodingInfo = createEncodingInfo(bin, items);

			expect(encodingInfo.binDimensionsBitSize).toBe(BitSize.Eight);
			expect(encodingInfo.itemDimensionsBitSize).toBe(expectedBitSize)
			expect(encodingInfo.itemCoordinatesBitSize).toBe(BitSize.Eight)
		}
	);

	test.each(testData)("CreateEncodingInfo Creates EncodingInfo With Correct ItemCoordinatesBitSize",
		(minValue, maxValue, expectedBitSize) => {

			const bin = createFakeBin(0, Sizes.byteMaxSize);
			const items = createFakeItems(0, Sizes.byteMaxSize, minValue, maxValue,3);

			const encodingInfo = createEncodingInfo(bin, items);

			expect(encodingInfo.binDimensionsBitSize).toBe(BitSize.Eight);
			expect(encodingInfo.itemDimensionsBitSize).toBe(BitSize.Eight)
			expect(encodingInfo.itemCoordinatesBitSize).toBe(expectedBitSize)
		}
	);
});


