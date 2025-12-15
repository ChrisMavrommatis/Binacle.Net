import {Bin} from "./bin";
import {Item} from "./item";
import {PackingParameters} from "./packingParameters";

export interface PackingRequest {
	parameters: PackingParameters;
	bins: Bin[];
	items: Item[];
}

