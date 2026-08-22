import type { Alpine as AlpineType } from 'alpinejs';
import {
	defineComponent,
	getResponseStatusText,
	largestBin,
	randomBin,
	randomItemFor,
	randomSample
} from "../utils";
import {Bin, Item, Error} from "../viewModels";
import {PackingRequest, PackingResponse} from "../apiModels";
import {PackedData} from "../apiModels/packingResponse";

export function packingDemoAppPlugin(Alpine: AlpineType) {
	Alpine.data('packing_demo_app', packingDemoApp);
}

export interface PackingDemoOptions {
	// Empty means fetch relative, from whatever host is serving the page.
	baseUrl?: string;
}

export const packingDemoApp = defineComponent((options: PackingDemoOptions = {}) => ({
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
		const sample = randomSample();
		this.model.bins = sample.bins;
		this.model.items = sample.items;
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
	// The bin the items are sized against, and the one a new bin is copied from. A fresh roll when there are
	// none, so nothing here has to handle an empty list.
	sizingBin() {
		return this.model.bins.length > 0 ? largestBin(this.model.bins) : randomBin();
	},
	addBin: {
		['@click']() {
			// A copy, not a roll: a fourth candidate is only worth comparing if it keeps the same footprint.
			const last = this.model.bins[this.model.bins.length - 1];
			this.model.bins.push(last ? new Bin(last.length, last.width, last.height) : randomBin());
		}
	},
	clearAllBins: {
		['@click']() {
			this.model.bins = [];
		}
	},
	removeItem(index: number) {
		this.model.items.splice(index, 1);
	},
	addItem: {
		['@click']() {
			this.model.items.push(randomItemFor(this.sizingBin(), 1));
		}
	},
	clearAllItems: {
		['@click']() {
			this.model.items = [];
		}
	},
	// One button, because two independent rolls are the impossible-pair bug this replaced.
	randomize: {
		['@click']() {
			const sample = randomSample();
			this.model.bins = sample.bins;
			this.model.items = sample.items;
		}
	},
	async handleErrorResponse(response: Response) {
		let errorObj = {
			title: `Error: ${response.statusText || getResponseStatusText(response.status) || response.status}`,
			errors: []
		} as Error;

		try {
			const responseJson = await response.json();
			if(responseJson?.title){
				errorObj.title = responseJson.title;
			}
			if(responseJson?.detail){
				errorObj.errors.push(responseJson.detail);
			}
			if(response.status === 422 && responseJson?.errors){
				for(const key in responseJson.errors){
					const fieldErrors = responseJson.errors[key];
					fieldErrors.forEach((err: string) => {
						errorObj.errors.push(`${key}: ${err}`);
					});
				}
			}
		}
		catch (error) {
			this.$logger.error("[Binacle] Error while parsing error response", error);
			errorObj.errors.push('An error occurred, but the error response could not be parsed.');
		}
		this.$dispatch('error-occurred', errorObj);
	},
	async getResults(request: PackingRequest) : Promise<PackingResponse | null> {
		try {
			const response = await fetch(`${options.baseUrl ?? ''}/api/v3/pack/by-custom`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify(request)
			})
			if(response.status === 200){
				const responseJson = await response.json();
				return responseJson as PackingResponse;
			}
			await this.handleErrorResponse(response);
			return null;
		} catch (error) {
			this.$logger.error("[Binacle] Error while fetching packing results", error);
			this.$dispatch('error-occurred', {
				title: "Error while fetching packing results",
				errors: [error instanceof Error ? error.message : String(error)]
			});
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
