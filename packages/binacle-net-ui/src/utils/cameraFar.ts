import {Dimensions} from "../models";

export function cameraFar(bin: Dimensions | null) {
	if (!bin) {
		return 1000;
	}
	const distance = Math.sqrt((bin.length ** 2) + (bin.height ** 2) + (bin.width ** 2));
	const far = (distance * 2) + (bin.height * 2);
	const roundedFar = Math.ceil(far);
	return roundedFar;
}
