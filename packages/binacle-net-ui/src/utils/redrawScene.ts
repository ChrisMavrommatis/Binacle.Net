import {clearItemsFromScene} from "./clearItemsFromScene";
import {clearBinFromScene} from "./clearBinFromScene";
import {getCameraPosition} from "./getCameraPosition";
import {getBinOrigin} from "./getBinOrigin";
import {createItem} from "./createItem";
import { PerspectiveCamera, Scene } from "three";
import {Coordinates, Dimensions} from "../models";
import {cameraFar} from "./cameraFar";
import {createBin} from "./createBin";
import {_itemMaterial} from "./_itemMaterial";

export function redrawScene(
	scene: Scene,
	camera: PerspectiveCamera,
	bin: Dimensions,
	packedItems: (Dimensions & Coordinates)[] | null) {
	clearItemsFromScene(scene);
	clearBinFromScene(scene);

	const cameraPosition = getCameraPosition(bin);
	camera.position.set(cameraPosition.x, cameraPosition.y, cameraPosition.z);

	camera.far = cameraFar(bin);
	const bin3D = createBin(bin);
	scene.add(bin3D);

	if (!packedItems || packedItems.length < 1) {
		return;
	}

	const binOrigin = getBinOrigin(bin);

	for (let i = 0; i < packedItems.length; i++) {
		let item = createItem(packedItems[i], i, _itemMaterial, binOrigin);
		scene.add(item);
	}

	camera.updateProjectionMatrix();
}
