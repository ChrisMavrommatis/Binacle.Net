import {Dimensions} from "../models";

export function getCameraPosition(bin: Dimensions) {
	let longestDimension = bin.length;

	if (bin.height > longestDimension) {
		longestDimension = bin.height;
	}
	if (bin.width > longestDimension) {
		longestDimension = bin.width;
	}

	let cameraPositionX = longestDimension;
	let cameraPositionY = longestDimension;
	let cameraPositionZ = longestDimension * 1.2;

	// if square
	if (bin.length === bin.width && bin.width === bin.height) {
		cameraPositionX = longestDimension;
		cameraPositionY = longestDimension;
		cameraPositionZ = longestDimension * 2;
	}

	return {
		x: cameraPositionX,
		y: cameraPositionY,
		z: cameraPositionZ
	}
}
