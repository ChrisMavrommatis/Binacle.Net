import {BoxGeometry, EdgesGeometry, LineSegments, Scene } from "three";

export function getBin(scene: Scene) {
	const binObject = scene.getObjectByName('bin') as LineSegments;
	if (!binObject) {
		return null;
	}
	const binGeometry = binObject.geometry as EdgesGeometry;
	const boxGeometry = binGeometry.parameters.geometry as BoxGeometry;
	return {
		length: boxGeometry.parameters.width,
		width: boxGeometry.parameters.depth,
		height: boxGeometry.parameters.height
	};
}
