import { Scene } from "three";

export function removeItemFromScene(scene: Scene, index: number) {
	const item = scene.getObjectByName(`item_${index}`);
	if (item) {
		scene.remove(item);
	}
}
