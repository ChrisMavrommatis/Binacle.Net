import Bin from '../models/bin.js'
import Item from '../models/item.js'

function getRandomInt(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

export default () => ({
	model: {
		bins: [],
		items: [],
		algorithm: '',
	},
	isValid(){
		const binsValid = this.model.bins.every(bin => !bin.hasErrors());
		const itemsValid = this.model.items.every(item => !item.hasErrors());
		return binsValid && itemsValid;
	},
	algorithms: [
		{ value: 'FFD', text: 'First Fit Decreasing' },
		{ value: 'BFD', text: 'Best Fit Decreasing' },
		{ value: 'WFD', text: 'Worst Fit Decreasing' },
	],
	removeBin(index) {
		this.model.bins.splice(index, 1);
	},
	addBin: {
		['@click']() {
			// random 1-60
			const length = getRandomInt(10, 60);
			const width = getRandomInt(10, 60);
			const height = getRandomInt(10, 60);
			this.model.bins.push(new Bin(length, width, height));
		}
	},
	clearAllBins:{
		['@click']() {
			// random 1-60
			this.model.bins = [];
		}
	},
	randomizeBinsFromSamples:{
		['@click']() {
			this.model.bins = [
				new Bin(60, 40, 30),
				new Bin(60, 40, 40),
				new Bin(60, 40, 60),
			];
		}
	},
	removeItem(index) {
		this.model.items.splice(index, 1);
	},
	addItem:{
		['@click']() {
			const length = getRandomInt(10, 60);
			const width = getRandomInt(10, 60);
			const height = getRandomInt(10, 60);
			this.model.items.push(new Item(length, width, height, 1));
		}
	},
	clearAllItems:{
		['@click']() {
			this.model.items = [];
		}
	},
	randomizeItemsFromSamples:{
		['@click']() {
			this.model.items = [
				new Item(2, 5, 10, 7),
				new Item(12, 15, 10, 3),
				new Item(10, 15, 15, 2)
			];
		}
	},
	init() {
		this.model.bins = [
			new Bin(60, 40, 30),
			new Bin(60, 40, 40),
			new Bin(60, 40, 60),
		];
		this.model.items = [
			new Item(2, 5, 10, 7),
			new Item(12, 15, 10, 3),
			new Item(10, 15, 15, 2)
		];
		this.model.algorithm = this.algorithms.at(0).value;
	},
	getResults:{
		['@click.prevent.stop']() {
			if(!this.isValid()){
				console.error("model is not valid")
				return;
			}

			const request = {
				parameters: {
					algorithm: this.model.algorithm,
				},
				bins: this.model.bins.map(x => ({
					id: x.id,
					length: x.length,
					width: x.width,
					height: x.height
				})),
				items: this.model.items.map(x => ({
					id: x.id,
					length: x.length,
					width: x.width,
					height: x.height,
					quantity: x.quantity
				})),
			}

			this.$dispatch('get-results', request);
		}
	}
});
