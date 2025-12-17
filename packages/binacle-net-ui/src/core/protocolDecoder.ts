import type { Alpine as AlpineType } from 'alpinejs';

import {defineComponent} from "../utils";
import {ViPaqSerializer} from "binacle-vipaq";
import {DecodedPackingResult} from "../viewModels";

export function protocolDecoderAppPlugin(Alpine: AlpineType) {
	Alpine.data('protocol_decoder_app', protocolDecoderApp);
}

export const protocolDecoderApp = defineComponent(() => ({
	model: {
		result: null as string | null
	},
	results: [] as DecodedPackingResult[],
	selectedResult: null as DecodedPackingResult | null,
	init(){
		const savedResults = localStorage.getItem('ProtocolDecoderSavedResults');
		if(savedResults) {
			const encodedResults = JSON.parse(savedResults) as string[];
			encodedResults.forEach(encodedResult => {
				const data = Uint8Array.from(atob(encodedResult), x => x.charCodeAt(0));
				ViPaqSerializer.deserialize(data)
					.then(result => {
						const decodedResult = new DecodedPackingResult(encodedResult, result.bin, result.items);
						this.results.push(decodedResult);
						if (this.results.length === 1) {
							this.selectResult(decodedResult);
						}
					})
					.catch(error => {
						this.$dispatch('error-occurred', ['Error deserializing saved ViPaq data', error]);
					});
			});
		}
	},
	addResult(){
		if (!this.model.result) {
			this.$dispatch('error-occurred', ['No ViPaq data to deserialize']);
			return;
		}

		const found = this.results.find(x => x.encodedResult === this.model.result);
		if(found){
			this.$dispatch('error-occurred', ['This ViPaq data has already been added']);
			this.model.result = null;
			return;
		}

		try {
			const data = Uint8Array.from(atob(this.model.result), x => x.charCodeAt(0));

			ViPaqSerializer.deserialize(data)
				.then(result => {
					const decodedResult = new DecodedPackingResult(this.model.result!, result.bin, result.items);

					this.$logger.info("[Binacle] ViPaq data", result);
					this.results.push(decodedResult);
					if (this.results.length === 1) {
						this.selectResult(decodedResult);
					}
					this.model.result = null;
					localStorage.setItem('ProtocolDecoderSavedResults', JSON.stringify(this.results.map(r => r.encodedResult)));
				})
				.catch(error => {
					this.$dispatch('error-occurred', ['Error deserializing ViPaq data', error]);
				});
		} catch (error) {
			this.$dispatch('error-occurred', ['Error deserializing ViPaq data', error]);
		}

	},
	isSelected(result: DecodedPackingResult): boolean {
		return this.selectedResult === result;
	},
	selectResult(result: DecodedPackingResult | null){
		this.selectedResult = result;
		this.$dispatch('update-scene', async () => {
			return result;
		});
	},
	deleteResult(result: DecodedPackingResult){
		const isSelected = this.isSelected(result);
		const index = this.results.indexOf(result);
		if(index !== -1){
			this.results.splice(index, 1);
			localStorage.setItem('ProtocolDecoderSavedResults', JSON.stringify(this.results.map(r => r.encodedResult)));
			if(isSelected) {
				this.selectResult(this.results.length > 0 ? this.results[0] : null);
			}
		}
	},
	resultTitle(result: DecodedPackingResult){
		return `Bin: ${result.bin.length}x${result.bin.width}x${result.bin.height}`;
	},
	resultBinPercentageText(result: DecodedPackingResult){
		return `Packed Bin Volume: ${result.packedBinVolumePercentage()}%`;
	}

}));
