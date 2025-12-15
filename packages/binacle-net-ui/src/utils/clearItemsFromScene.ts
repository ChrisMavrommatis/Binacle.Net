import {Scene} from "three";

export function clearItemsFromScene(scene: Scene) {
	const items = scene.getObjectsByProperty("typeName", "item");
	for (let i = 0; i < items.length; i++) {
		scene.remove(items[i]);
	}
}

