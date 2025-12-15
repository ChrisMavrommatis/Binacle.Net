import {Coordinates} from "../models";

export function getMeshPosition(binOrigin: Coordinates, itemOrigin: Coordinates, packedItem: Coordinates) {
	return {
		x: binOrigin.x + itemOrigin.x + packedItem.x,
		y: binOrigin.y + itemOrigin.y + packedItem.z,
		z: binOrigin.z + itemOrigin.z + packedItem.y,
	};
}
