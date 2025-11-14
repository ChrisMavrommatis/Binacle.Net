import {getRandomInt} from "../utils/math";
import Bin from '../models/bin.js'
import Item from '../models/item.js'


function getRandomBin(){
	const length = getRandomInt(10, 60);
	const width = getRandomInt(10, 60);
	const height = getRandomInt(10, 60);
	return new Bin(length, width, height);
}
function getRandomItem(quantityOverride = null){
	const length = getRandomInt(5, 30);
	const width = getRandomInt(5, 30);
	const height = getRandomInt(5, 30);
	const quantity = quantityOverride || getRandomInt(1, 10);
	return new Item(length, width, height, quantity);
}

export default (base_url) => ({
	model: {
		bins: [],
		items: [],
		algorithm: '',
	},
	algorithms: [
		{value: 'FFD', text: 'First Fit Decreasing'},
		{value: 'BFD', text: 'Best Fit Decreasing'},
		{value: 'WFD', text: 'Worst Fit Decreasing'},
	],
	results: [],
	selectedResult: null,
	init() {
		this.model.bins = [
			new Bin(60, 40, 10),
			new Bin(60, 40, 20),
			new Bin(60, 40, 30),
		];
		this.model.items = [
			new Item(2, 5, 10, 7),
			new Item(12, 15, 10, 3),
			new Item(10, 15, 15, 2)
		];
		this.model.algorithm = this.algorithms.at(0).value;
	},
	isValid() {
		const binsValid = this.model.bins.every(bin => !bin.hasErrors());
		const itemsValid = this.model.items.every(item => !item.hasErrors());
		return binsValid && itemsValid;
	},
	removeBin(index) {
		this.model.bins.splice(index, 1);
	},
	addBin: {
		['@click']() {
			this.model.bins.push(
				getRandomBin()
			);
		}
	},
	clearAllBins: {
		['@click']() {
			this.model.bins = [];
		}
	},
	randomizeBins: {
		['@click']() {
			const noOfVariedBins = getRandomInt(2, 5);
			const bins = [];
			for (let i = 0; i < noOfVariedBins; i++) {
				bins.push(
					getRandomBin()
				);
			}
			this.model.bins = bins;
		}
	},
	removeItem(index) {
		this.model.items.splice(index, 1);
	},
	addItem: {
		['@click']() {
			this.model.items.push(
				getRandomItem(1)
			);
		}
	},
	clearAllItems: {
		['@click']() {
			this.model.items = [];
		}
	},
	randomizeItems: {
		['@click']() {
			const noOfVariedItems = getRandomInt(3, 7);
			const items = [];
			for (let i = 0; i < noOfVariedItems; i++) {
				items.push(
					getRandomItem()
				);
			}
			this.model.items = items;
		}
	},
	async getResults(request){
		const response = await fetch(`${base_url}/api/v3/pack/by-custom`, {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		});
		return await response.json();
	},
	onSubmit() {
		if (!this.isValid()) {
			this.$logger.error("[Binacle] Model is not valid");
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
			}))
		};

		this.$dispatch('update-scene', async () =>
		{
			this.$logger.log('[Binacle] Packing request sent', request);
			const response = await this.getResults(request);
			this.$logger.log('[Binacle] Packing results received', response);
			const firstSuccessfulResult = response.data.find(x => !!x.bin);
			this.results = response.data;
			this.selectResult(firstSuccessfulResult);
			return firstSuccessfulResult;
		});

	},
	isSelected(result){
		return this.selectedResult === result;
	},
	selectResult(result) {
		this.selectedResult = result;
		this.$dispatch('update-scene', async () =>
		{
			return result;
		});
	},
	colorClass(result){
		if(result.result === 'FullyPacked'){
			return 'green';
		}
		if(result.result === 'PartiallyPacked'){
			return 'orange';
		}
		return 'red';
	},
	resultTitle(result) {
		return `Bin: ${result.bin.id}`;
	},
	resultBinPercentageText(result){
		return `Packed Bin Volume: ${result.packedBinVolumePercentage}%`;
	},
	resultItemPercentageText(result) {
		return `Packed Items Volume: ${result.packedItemsVolumePercentage}%`
	},
	resultIsFullyPacked(result){
		return result.result === 'FullyPacked';
	}
});


