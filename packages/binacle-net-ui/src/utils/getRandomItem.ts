import {Item} from "../viewModels";
import {getRandomInt} from "./getRandomInt";

export function getRandomItem(quantityOverride: number | null) {
	const length = getRandomInt(5, 30);
	const width = getRandomInt(5, 30);
	const height = getRandomInt(5, 30);
	const quantity = quantityOverride || getRandomInt(1, 10);
	return new Item(length, width, height, quantity);
}
