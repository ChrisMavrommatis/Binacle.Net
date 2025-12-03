import {ViPaqSerializer} from "binacle-vipaq";
import DecodedPackingResult from "../models/decodedPackingResult";

export default () => ({
	model: {
		result: null
	},
	results: [],
	selectedResult: null,
	init(){
		const savedResults = localStorage.getItem('ProtocolDecoderSavedResults');
		if(savedResults) {
			const encodedResults = JSON.parse(savedResults);
			encodedResults.forEach(encodedResult => {
				const data = Uint8Array.fromBase64(encodedResult);
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
	addResult() {
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
			const data = Uint8Array.fromBase64(this.model.result);

			ViPaqSerializer.deserialize(data)
				.then(result => {
					const decodedResult = new DecodedPackingResult(this.model.result, result.bin, result.items);

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
	isSelected(result) {
		return this.selectedResult === result;
	},
	selectResult(result) {
		this.selectedResult = result;
		this.$dispatch('update-scene', async () => {
			return result;
		});
	},
	resultTitle(result) {
		return `Bin: ${result.bin.length}x${result.bin.width}x${result.bin.height}`;
	},
	resultBinPercentageText(result) {
		return `Packed Bin Volume: ${result.packedBinVolumePercentage()}%`;
	},
});
