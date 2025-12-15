import {Bin} from "../viewModels";
import {getRandomInt} from "./getRandomInt";

export function getRandomBin() {
	const length = getRandomInt(10, 60);
	const width = getRandomInt(10, 60);
	const height = getRandomInt(10, 60);
	return new Bin(length, width, height);
}
