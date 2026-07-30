import type { Alpine as AlpineType } from 'alpinejs';

import {defineComponent} from "../utils";
import {ViPaqSerializer} from "binacle-vipaq";
import {DecodedPackingResult} from "../viewModels";

const SAVED_RESULTS_KEY = 'ProtocolDecoderSavedResults';

// Bump when the ViPaq wire changes. "2" is the rebuilt wire (PROTOCOL.md). The stored value carries its own
// version; anything without a matching version — including the old bare array of tokens — is from a previous
// format and cannot be decoded, so it is discarded on load.
const CURRENT_SCHEMA_VERSION = 2;

interface SavedResults {
	version: number;
	results: string[];
}

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
		this.loadSavedResults().forEach(encodedResult => {
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
	},
	// Reads the stored tokens, but only if they carry the current schema version. Anything else — the old bare
	// array, an older version, or corrupt JSON — is from a previous ViPaq wire and cannot be decoded, so it is
	// discarded and the user is told once.
	loadSavedResults(): string[] {
		const raw = localStorage.getItem(SAVED_RESULTS_KEY);
		if (!raw) {
			return [];
		}

		try {
			const parsed = JSON.parse(raw) as Partial<SavedResults>;
			if (parsed?.version === CURRENT_SCHEMA_VERSION && Array.isArray(parsed.results)) {
				return parsed.results;
			}
		} catch {
			// not valid JSON — fall through and treat as stale
		}

		this.$dispatch('error-occurred', ['Your saved results were cleared: the packing token format changed and the old saved tokens can no longer be decoded.']);
		localStorage.removeItem(SAVED_RESULTS_KEY);
		return [];
	},
	// Persists the current tokens under the current schema version.
	saveResults(){
		const payload: SavedResults = {
			version: CURRENT_SCHEMA_VERSION,
			results: this.results.map(r => r.encodedResult)
		};
		localStorage.setItem(SAVED_RESULTS_KEY, JSON.stringify(payload));
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
					this.saveResults();
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
			this.saveResults();
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
