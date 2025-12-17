import {Alpine as AlpineType} from 'alpinejs'
import {loggerPlugin, packingVisualizerPlugin, errorsDialogPlugin, protocolDecoderAppPlugin} from "./core";

export function protocolDecoderPlugin(Alpine: AlpineType) {
	Alpine.plugin(loggerPlugin);
	Alpine.plugin(packingVisualizerPlugin);
	Alpine.plugin(protocolDecoderAppPlugin);
	Alpine.plugin(errorsDialogPlugin);
}


