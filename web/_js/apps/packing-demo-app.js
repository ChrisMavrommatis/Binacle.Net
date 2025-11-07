import Bin from '../models/bin.js'
import Item from '../models/item.js'

export default () => ({
	bins: [],
	items: [],
	removeBin(index) {
		this.bins.splice(index, 1);
	},
	addBin() {
		this.bins.push(new Bin(0, 0, 0));
	},
	clearAllBins() {
		this.bins = [];
	},
	randomizeBinsFromSamples() {
		this.bins = [
			new Bin(60, 40, 30),
			new Bin(60, 40, 40),
			new Bin(60, 40, 60),
		];
	},
	removeItem(index) {
		this.items.splice(index, 1);
	},
	addItem() {
		this.items.push(new Item(0, 0, 0, 1));
	},
	clearAllItems() {
		this.items = [];
	},
	randomizeItemsFromSamples() {
		this.items = [
			new Item(2, 5, 10, 7),
			new Item(12, 15, 10, 3),
			new Item(10, 15, 15, 2)
		];
	},
	init() {
		this.randomizeBinsFromSamples();
		this.randomizeItemsFromSamples();
	}
});
