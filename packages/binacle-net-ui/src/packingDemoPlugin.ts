import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin, fieldPlugin, packingVisualizerPlugin, errorsDialogPlugin, packingDemoAppPlugin} from "./core";

export function packingDemoPlugin(Alpine: AlpineType) {
	Alpine.plugin(fieldPlugin);
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packingDemoAppPlugin);
	Alpine.plugin(packingVisualizerPlugin);
	Alpine.plugin(errorsDialogPlugin);
}


