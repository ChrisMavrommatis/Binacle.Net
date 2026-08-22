import {Bin, Item} from "../../src/viewModels";
import {largestBin, randomBin, randomItemFor, randomSample, Sample} from "../../src/utils/samples";

// The rolled numbers, restated here so a change to samples.ts has to be a deliberate change to the test.
const minBinSide = 30;
const maxBinSide = 60;
const minItemSide = 8;
const maxQuantity = 10;

// Enough rolls that a one-in-a-thousand break shows up, few enough that the file stays under a second.
const rolls = 5000;

function volume(box: {length: number; width: number; height: number}) {
	return box.length * box.width * box.height;
}

// Rolls only. It asserts nothing, so each test keeps its own act and assert apart.
function rollSamples(count: number): Sample[] {
	const samples = [] as Sample[];
	for (let i = 0; i < count; i++) {
		samples.push(randomSample());
	}
	return samples;
}

function fitsIn(bin: Bin, items: Item[]) {
	const sideFits = items.every(
		item => item.length <= bin.length && item.width <= bin.width && item.height <= bin.height
	);
	const volumeFits = items.reduce((sum, item) => sum + volume(item) * item.quantity, 0) <= volume(bin);
	return sideFits && volumeFits;
}

// Feeds getRandomInt a fixed sequence. getRandomInt calls Math.random exactly once, so one value per roll.
function stubRandom(values: number[]) {
	let index = 0;
	jest.spyOn(Math, "random").mockImplementation(() => values[index++]);
}

afterEach(() => {
	jest.restoreAllMocks();
});

describe("randomBin", () => {
	test("every side is a whole number inside the bin range", () => {
		const bins = Array.from({length: rolls}, () => randomBin());

		const sides = bins.flatMap(bin => [bin.length, bin.width, bin.height]);

		const outOfRange = sides.filter(side => !Number.isInteger(side) || side < minBinSide || side > maxBinSide);
		expect(outOfRange).toEqual([]);
	});
});

describe("largestBin", () => {
	test("picks the bin with the greatest volume", () => {
		const bins = [new Bin(30, 30, 30), new Bin(60, 50, 40), new Bin(40, 40, 40)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[1]);
	});

	test("keeps the first bin when two share the greatest volume", () => {
		const bins = [new Bin(40, 30, 20), new Bin(20, 30, 40)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[0]);
	});

	test("returns the only bin of a single-bin set", () => {
		const bins = [new Bin(31, 32, 33)];

		const largest = largestBin(bins);

		expect(largest).toBe(bins[0]);
	});
});

describe("randomItemFor", () => {
	test("no side is more than half the matching bin side", () => {
		const bin = new Bin(60, 45, 31);

		const items = Array.from({length: rolls}, () => randomItemFor(bin, 1));

		const oversized = items.filter(
			item =>
				item.length > Math.floor(bin.length / 2) ||
				item.width > Math.floor(bin.width / 2) ||
				item.height > Math.floor(bin.height / 2)
		);
		expect(oversized).toEqual([]);
	});

	test("keeps the quantity it was given", () => {
		const bin = new Bin(50, 50, 50);

		const item = randomItemFor(bin, 7);

		expect(item.quantity).toBe(7);
	});
});

describe("randomSample", () => {
	test("rolls between two and five bins", () => {
		const samples = rollSamples(rolls);

		const counts = samples.map(sample => sample.bins.length);

		const outOfRange = counts.filter(count => count < 2 || count > 5);
		expect(outOfRange).toEqual([]);
	});

	test("rolls between two and four item types, never none", () => {
		const samples = rollSamples(rolls);

		const counts = samples.map(sample => sample.items.length);

		const outOfRange = counts.filter(count => count < 2 || count > 4);
		expect(outOfRange).toEqual([]);
	});

	test("every quantity is a whole number between one and ten", () => {
		const samples = rollSamples(rolls);

		const quantities = samples.flatMap(sample => sample.items.map(item => item.quantity));

		const outOfRange = quantities.filter(
			quantity => !Number.isInteger(quantity) || quantity < 1 || quantity > maxQuantity
		);
		expect(outOfRange).toEqual([]);
	});

	test("every item side is a whole number of at least eight", () => {
		const samples = rollSamples(rolls);

		const sides = samples.flatMap(sample => sample.items.flatMap(item => [item.length, item.width, item.height]));

		const tooSmall = sides.filter(side => !Number.isInteger(side) || side < minItemSide);
		expect(tooSmall).toEqual([]);
	});

	// The contract of the whole file: a roll can never hand the demo a set the largest bin cannot hold.
	// It is the largest bin only - see the next test.
	test("the item set always fits the largest bin", () => {
		const samples = rollSamples(rolls);

		const impossible = samples.filter(sample => !fitsIn(largestBin(sample.bins), sample.items));

		expect(impossible).toEqual([]);
	});

	// The smaller bins are meant to fail. A test that asserted the set fits every bin would be asserting the
	// demo page has nothing to show.
	test("the smaller bins are often too small, which is the point of the page", () => {
		const samples = rollSamples(rolls);

		const withAFailingBin = samples.filter(sample => {
			const largest = largestBin(sample.bins);
			return sample.bins.some(bin => bin !== largest && !fitsIn(bin, sample.items));
		});

		expect(withAFailingBin.length).toBeGreaterThan(0);
	});

	// itemsFor divides the volume budget by the types still to come, so a near-maximum first item can floor to
	// zero and be skipped. Pinned rather than left to chance: it is about one roll in 50,000.
	test("drops an item type whose quantity floors to zero", () => {
		stubRandom([
			0, // two bins
			0.99, 0.99, 0.99, // 60x60x60, the largest
			0, 0, 0, // 30x30x30
			0.99, // four item types
			0, // budget of 45 percent
			0.99, 0.99, 0.99, // 30x30x30 item, 27000, over a quarter of the 97200 budget - dropped
			0, 0, 0, // 8x8x8
			0, 0, 0, // 8x8x8
			0, 0, 0, // 8x8x8
		]);

		const sample = randomSample();

		expect(sample.items).toHaveLength(3);
	});
});
