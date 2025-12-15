import {Scene} from "three";

export function clearBinFromScene(scene: Scene) {
	const bin = scene.getObjectByName('bin');
	if (bin) {
		scene.remove(bin);
	}
}
