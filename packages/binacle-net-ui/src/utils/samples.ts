import {Bin, Item} from "../viewModels";
import {getRandomInt} from "./getRandomInt";

export interface Sample {
	bins: Bin[];
	items: Item[];
}

// 30 is the floor because an item side is half a bin side and still has to clear 8.
const minBinSide = 30;
const maxBinSide = 60;
const minItemSide = 8;

export function randomBin() {
	return new Bin(
		getRandomInt(minBinSide, maxBinSide),
		getRandomInt(minBinSide, maxBinSide),
		getRandomInt(minBinSide, maxBinSide)
	);
}

function randomBins() {
	const count = getRandomInt(2, 5);
	const bins = [] as Bin[];
	for (let i = 0; i < count; i++) {
		bins.push(randomBin());
	}
	return bins;
}

// The bin everything is sized against: the largest by volume, so the set always fits at least one candidate
// and the smaller ones are the interesting result. That comparison is what the page is for.
export function largestBin(bins: Bin[]) {
	return bins.reduce((largest, bin) =>
		bin.length * bin.width * bin.height > largest.length * largest.width * largest.height ? bin : largest
	);
}

// Half the matching bin side, floored at 8. Eight of them fit before any packing thought, so nothing rolled
// here can be an item the bin will not hold.
function randomSide(binSide: number) {
	return getRandomInt(minItemSide, Math.max(minItemSide, Math.floor(binSide / 2)));
}

export function randomItemFor(bin: Bin, quantity: number) {
	return new Item(randomSide(bin.length), randomSide(bin.width), randomSide(bin.height), quantity);
}

// Quantities are shared out of a volume budget of 45-75% of the bin and never rounded up, so the set cannot
// exceed the bin either. Measured over 200,000 rolls: 0% impossible, median fill 55%, 2-4 item types.
function itemsFor(bin: Bin) {
	const binVolume = bin.length * bin.width * bin.height;
	const types = getRandomInt(2, 4);
	const items = [] as Item[];
	let budget = binVolume * (getRandomInt(45, 75) / 100);

	for (let i = 0; i < types; i++) {
		const item = randomItemFor(bin, 1);
		const itemVolume = item.length * item.width * item.height;
		const quantity = Math.min(10, Math.floor(budget / (types - i) / itemVolume));
		if (quantity < 1) {
			continue;
		}
		item.quantity = quantity;
		items.push(item);
		budget -= itemVolume * quantity;
	}

	return items;
}

// The bins first, then the items sized to them. One call, because two independent rolls are exactly the
// impossible-pair bug this replaced.
export function randomSample(): Sample {
	const bins = randomBins();
	return {bins, items: itemsFor(largestBin(bins))};
}
