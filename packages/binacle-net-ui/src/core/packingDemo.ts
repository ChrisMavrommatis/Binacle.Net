import type { Alpine as AlpineType } from 'alpinejs';
import {defineComponent, getRandomBin, getRandomInt, getRandomItem} from "../utils";
import {Bin, Item} from "../viewModels";
import {PackingParameters} from "../apiModels/packingParameters";
import {PackingRequest} from "../apiModels/packingRequest";
import {PackingResponse} from "../apiModels";
import {PackedData} from "../apiModels/packingResponse";

export function packingDemoAppPlugin(Alpine: AlpineType) {
	Alpine.data('packing_demo_app', packingDemoApp);
}

export const packingDemoApp = defineComponent((base_url: string) => ({
	model: {
		bins: [] as Bin[],
		items: [] as Item[],
		algorithm: '',
	},
	algorithms: [
		{value: 'FFD', text: 'First Fit Decreasing'},
		{value: 'BFD', text: 'Best Fit Decreasing'},
		{value: 'WFD', text: 'Worst Fit Decreasing'},
	],
	results: [] as PackedData[],
	selectedResult: null as PackedData | null,
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
		this.model.algorithm = this.algorithms[0].value;
	},
	isValid() {
		const binsValid = this.model.bins.every(bin => !bin.hasErrors());
		const itemsValid = this.model.items.every(item => !item.hasErrors());
		return binsValid && itemsValid;
	},
	removeBin(index: number) {
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
			const bins = [] as Bin[];
			for (let i = 0; i < noOfVariedBins; i++) {
				bins.push(
					getRandomBin()
				);
			}
			this.model.bins = bins;
		}
	},
	removeItem(index: number) {
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
			const items = [] as Item[];
			for (let i = 0; i < noOfVariedItems; i++) {
				items.push(
					getRandomItem(null)
				);
			}
			this.model.items = items;
		}
	},
	async getResults(request: PackingRequest) : Promise<PackingResponse | null> {
		try {
			const response = await fetch(`${base_url}/api/v3/pack/by-custom`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify(request)
			})
			const responseJson = await response.json();

			// TODO: Handle errors properly
			// if(response.status !== 200){
			// 	let event = {
			// 		errors: []
			// 	};
			// 	event.errors.push(`Error: ${response.status}.`);
			// 	if(responseJson?.title){
			// 		event.title = responseJson.title;
			// 	}
			// 	if(responseJson?.detail){
			// 		event.errors.push(responseJson.detail);
			// 	}
			// 	if(response.status == 422){
			// 		for(const key in responseJson?.errors){
			// 			const fieldErrors = responseJson.errors[key];
			// 			fieldErrors.forEach(err => {
			// 				event.errors.push(`${key}: ${err}`);
			// 			});
			// 		}
			// 	}
			// 	this.$dispatch('error-occurred', event);
			// 	return null;
			// }
			return responseJson as PackingResponse;
		} catch (error) {
			this.$logger.error("[Binacle] Error while fetching packing results", error);
			this.$dispatch('error-occurred', ['Error while fetching packing results. Please try again later.']);
			return null;
		}
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
		} as PackingRequest;

		this.$dispatch('update-scene', async () => {
			this.$logger.log('[Binacle] Packing request sent', request);
			const response = await this.getResults(request);
			this.$logger.log('[Binacle] Packing results received', response);
			if(!response || !response.data){
				this.results = [];
				this.selectedResult = null;
				return null;
			}
			const firstSuccessfulResult = response.data.find(x => !!x.bin);
			this.results = response.data;
			this.selectedResult = firstSuccessfulResult || null;
			return {
				bin: firstSuccessfulResult?.bin,
				items: firstSuccessfulResult?.packedItems || []
			};
		});

	},
	isSelected(result: PackedData) {
		return this.selectedResult === result;
	},
	selectResult(result: PackedData) {
		this.selectedResult = result;
		this.$dispatch('update-scene', async () => {
			return {
				bin: result?.bin,
				items: result?.packedItems || []
			};
		});
	},
	colorClass(result: PackedData) {
		if (result.result === 'FullyPacked') {
			return 'green';
		}
		if (result.result === 'PartiallyPacked') {
			return 'orange';
		}
		return 'red';
	},
	resultTitle(result: PackedData) {
		return `Bin: ${result.bin.id}`;
	},
	resultBinPercentageText(result: PackedData) {
		return `Packed Bin Volume: ${result.packedBinVolumePercentage}%`;
	},
	resultItemPercentageText(result: PackedData) {
		return `Packed Items Volume: ${result.packedItemsVolumePercentage}%`
	},
	resultIsFullyPacked(result: PackedData) {
		return result.result === 'FullyPacked';
	}
}));
