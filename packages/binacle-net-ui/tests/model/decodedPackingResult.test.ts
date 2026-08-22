import DecodedPackingResult from "../../src/viewModels/decodedPackingResult";

function newResult(bin: {length: number; width: number; height: number}, items: number[][]) {
	return new DecodedPackingResult(
		"encoded",
		bin,
		items.map(([length, width, height]) => ({length, width, height, x: 0, y: 0, z: 0}))
	);
}

describe("binVolume", () => {
	test("multiplies the three bin sides", () => {
		const result = newResult({length: 10, width: 20, height: 30}, []);

		const volume = result.binVolume();

		expect(volume).toBe(6000);
	});
});

describe("itemsVolume", () => {
	test("is zero when nothing was packed", () => {
		const result = newResult({length: 10, width: 20, height: 30}, []);

		const volume = result.itemsVolume();

		expect(volume).toBe(0);
	});

	test("adds up every packed item", () => {
		const result = newResult({length: 10, width: 20, height: 30}, [[1, 2, 3], [4, 5, 6]]);

		const volume = result.itemsVolume();

		expect(volume).toBe(126);
	});

	// Each entry is one packed item. A quantity of five is five entries, so nothing here multiplies.
	test("counts a repeated item once per entry", () => {
		const result = newResult({length: 10, width: 20, height: 30}, [[2, 2, 2], [2, 2, 2]]);

		const volume = result.itemsVolume();

		expect(volume).toBe(16);
	});
});

describe("packedBinVolumePercentage", () => {
	test("is the item volume over the bin volume, as a whole number", () => {
		const result = newResult({length: 10, width: 10, height: 10}, [[5, 5, 5]]);

		const percentage = result.packedBinVolumePercentage();

		expect(percentage).toBe(13);
	});

	test("is zero for an empty bin", () => {
		const result = newResult({length: 10, width: 10, height: 10}, []);

		const percentage = result.packedBinVolumePercentage();

		expect(percentage).toBe(0);
	});

	test("is a hundred when the items fill the bin", () => {
		const result = newResult({length: 10, width: 10, height: 10}, [[10, 10, 10]]);

		const percentage = result.packedBinVolumePercentage();

		expect(percentage).toBe(100);
	});

	test("a zero-sided bin gives zero rather than NaN", () => {
		const result = newResult({length: 0, width: 10, height: 10}, []);

		const percentage = result.packedBinVolumePercentage();

		expect(percentage).toBe(0);
	});
});

describe("encodedResult", () => {
	test("is kept as given", () => {
		const result = newResult({length: 10, width: 10, height: 10}, []);

		const encoded = result.encodedResult;

		expect(encoded).toBe("encoded");
	});
});
