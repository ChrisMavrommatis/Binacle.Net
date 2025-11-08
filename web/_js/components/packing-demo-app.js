import Bin from '../models/bin.js'
import Item from '../models/item.js'

export default () => ({
	model: {
		bins: [],
		items: [],
		algorithm: '',
	},
	algorithms: [
		{ value: 'FFD', text: 'First Fit Decreasing' },
		{ value: 'BFD', text: 'Best Fit Decreasing' },
		{ value: 'WFD', text: 'Worst Fit Decreasing' },
	],
	removeBin(index) {
		this.model.bins.splice(index, 1);
	},
	addBin() {
		this.model.bins.push(new Bin(0, 0, 0));
	},
	clearAllBins() {
		this.model.bins = [];
	},
	randomizeBinsFromSamples() {
		this.model.bins = [
			new Bin(60, 40, 30),
			new Bin(60, 40, 40),
			new Bin(60, 40, 60),
		];
	},
	removeItem(index) {
		this.model.items.splice(index, 1);
	},
	addItem() {
		this.model.items.push(new Item(0, 0, 0, 1));
	},
	clearAllItems() {
		this.model.items = [];
	},
	randomizeItemsFromSamples() {
		this.model.items = [
			new Item(2, 5, 10, 7),
			new Item(12, 15, 10, 3),
			new Item(10, 15, 15, 2)
		];
	},
	init() {
		this.randomizeBinsFromSamples();
		this.randomizeItemsFromSamples();
		this.model.algorithm = this.algorithms.at(0).value;
	},
	getResults(){
		console.log(this.model);
	}
});
