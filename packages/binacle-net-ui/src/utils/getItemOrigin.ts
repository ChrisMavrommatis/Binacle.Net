import {Dimensions} from "../models";

export function getItemOrigin(packedItem: Dimensions) {
	return {
		x: packedItem.length / 2,
		y: packedItem.height / 2,
		z: packedItem.width / 2
	};
}
