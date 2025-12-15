import {Dimensions} from "../models";

export function getBinOrigin(bin: Dimensions) {
	return {
		x: -1 * bin.length / 2,
		y: -1 * bin.height / 2,
		z: -1 * bin.width / 2
	};
}

