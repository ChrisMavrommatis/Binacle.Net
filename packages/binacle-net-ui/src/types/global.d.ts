import { VisualizerState } from '../models/visualizerState';
import { Alpine as AlpineType } from 'alpinejs';

declare global {
	let Alpine: AlpineType;

	interface Window {
		binacle: {
			visualizerContainer: HTMLElement;
			rendererContainer: HTMLElement;
			visualizerState: VisualizerState | null;
		};
	}
}

export {};

