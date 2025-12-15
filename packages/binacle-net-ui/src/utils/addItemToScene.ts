import {Coordinates, Dimensions} from "../models";
import {getBinOrigin} from "./getBinOrigin";
import {createItem} from "./createItem";
import {Scene} from "three";
import {_itemMaterial} from "./_itemMaterial";

export function addItemToScene(scene: Scene, bin: Dimensions, packedItem: Dimensions & Coordinates, index: number) {
	const binOrigin = getBinOrigin(bin);

	let item = createItem(packedItem, index, _itemMaterial, binOrigin);

	scene.add(item);
}
