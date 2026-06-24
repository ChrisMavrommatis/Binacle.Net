import ViPaqSerializer from "../src/ViPaqSerializer";
import {Coordinates, Dimensions, Version} from "../src/models";
import {encodingInfoFromByte} from "../src/utils";

// Round trips the full serialize -> deserialize path. Nothing else in the suite does this,
// which is why the getByteSize and getCoordinatesBitSize bugs have gone unnoticed.
// Values stay at or below 32 bit because JS numbers cannot hold 64 bit integers exactly.

type Item = Dimensions & Coordinates;

function bin(length: number, width: number, height: number): Dimensions {
	return {length, width, height};
}

function item(length: number, width: number, height: number, x: number, y: number, z: number): Item {
	return {length, width, height, x, y, z};
}

async function roundTrip(input: Dimensions, items: Item[]) {
	const data = await ViPaqSerializer.serialize(input, items);
	const result = await ViPaqSerializer.deserialize(data);

	expect(result.bin.length).toBe(input.length);
	expect(result.bin.width).toBe(input.width);
	expect(result.bin.height).toBe(input.height);

	expect(result.items.length).toBe(items.length);
	for (let i = 0; i < items.length; i++) {
		expect(result.items[i].length).toBe(items[i].length);
		expect(result.items[i].width).toBe(items[i].width);
		expect(result.items[i].height).toBe(items[i].height);
		expect(result.items[i].x).toBe(items[i].x);
		expect(result.items[i].y).toBe(items[i].y);
		expect(result.items[i].z).toBe(items[i].z);
	}

	return {data, result};
}

describe('Serialization', () => {

	test('round trips when all values are eight bit', async () => {
		await roundTrip(bin(100, 110, 120), [
			item(10, 20, 30, 1, 2, 3),
			item(40, 50, 60, 10, 20, 30)
		]);
	});

	test('round trips when all values are sixteen bit', async () => {
		await roundTrip(bin(1000, 2000, 3000), [
			item(300, 400, 500, 600, 700, 800),
			item(900, 1000, 1100, 1200, 1300, 1400)
		]);
	});

	// Exposes the getByteSize bug: ThirtyTwo returns 3 instead of 4, so the buffer is too small
	// and the thirty two bit write runs off the end.
	test('round trips when values are thirty two bit', async () => {
		await roundTrip(bin(100_000, 200_000, 300_000), [
			item(70_000, 80_000, 90_000, 100_000, 110_000, 120_000)
		]);
	});

	// Exposes the getCoordinatesBitSize bug: coordinate 0 is rejected with `<= 0`.
	test('round trips when coordinates are zero', async () => {
		const {result} = await roundTrip(bin(100, 110, 120), [
			item(10, 20, 30, 0, 0, 0)
		]);
		expect(result.items[0].x).toBe(0);
		expect(result.items[0].y).toBe(0);
		expect(result.items[0].z).toBe(0);
	});

	// Exposes the getCoordinatesBitSize bug: it validates `width` instead of `z`, so a negative
	// z slips through and silently corrupts instead of being rejected.
	test('rejects a negative z coordinate', async () => {
		await expect(
			ViPaqSerializer.serialize(bin(100, 110, 120), [item(10, 20, 30, 1, 2, -1)])
		).rejects.toThrow();
	});

	test('compresses and round trips when the body is large', async () => {
		const items: Item[] = [];
		for (let i = 0; i < 60; i++) {
			items.push(item(10, 20, 30, 1, 2, 3));
		}

		const {data} = await roundTrip(bin(200, 200, 200), items);

		// 60 items is well over the 255 byte body threshold, so the payload must be gzipped.
		expect(encodingInfoFromByte(data[0]).version).toBe(Version.CompressedGzip);
	});

	test('round trips when there are no items', async () => {
		const {result} = await roundTrip(bin(100, 110, 120), []);
		expect(result.items.length).toBe(0);
		expect(result.bin.length).toBe(100);
	});

	test('rejects a null bin', async () => {
		await expect(
			ViPaqSerializer.serialize(null as unknown as Dimensions, [item(10, 20, 30, 1, 2, 3)])
		).rejects.toThrow();
	});

	test('rejects null items', async () => {
		await expect(
			ViPaqSerializer.serialize(bin(100, 110, 120), null as unknown as Item[])
		).rejects.toThrow();
	});

	test.each([
		[255, 0],   // BitSize.Eight
		[256, 1],   // BitSize.Sixteen
		[65_536, 2] // BitSize.ThirtyTwo
	])('chooses the correct bin dimensions width for dimension %i', async (dimension, expectedBitSize) => {
		const data = await ViPaqSerializer.serialize(bin(dimension, dimension, dimension), [item(1, 1, 1, 1, 1, 1)]);

		expect(encodingInfoFromByte(data[0]).binDimensionsBitSize).toBe(expectedBitSize);
	});
});
