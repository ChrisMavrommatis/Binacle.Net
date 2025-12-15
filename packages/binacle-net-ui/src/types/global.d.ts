import { VisualizerState } from '../models/visualizerState';
import { Alpine as AlpineType } from 'alpinejs';
import { Binacle as BinacleType } from '../core';

declare global {
	let Alpine: AlpineType;
	let Binacle: BinacleType;


	interface Window {
		binacle: BinacleType;
	}
}


export {};
