import {ViPaqSerializer} from "binacle-vipaq";
import DecodedPackingResult from "../models/decodedPackingResult";

export default () => ({
	model: {
		result: null
	},
	results: [],
	selectedResult: null,
	addResult() {
		if (!this.model.result) {
			// TODO: show error to user
			this.$logger.error("[Binacle] No ViPaq data to deserialize");
			return;
		}
		const data = Uint8Array.fromBase64(this.model.result);

		ViPaqSerializer.deserialize(data)
			.then(result => {
				const decodedResult = new DecodedPackingResult(this.model.result, result.bin, result.items);

				this.$logger.info("[Binacle] ViPaq data", result);
				this.results.push(decodedResult);
				this.selectResult(decodedResult);
			})
			.catch(error => {
				this.$logger.error("[Binacle] Error deserializing ViPaq data:", error);
			});
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
