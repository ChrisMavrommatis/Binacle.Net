import {BoxGeometry, Mesh, MeshNormalMaterial } from "three";
import {Coordinates, Dimensions} from "../models";
import {getItemOrigin} from "./getItemOrigin";
import {getMeshPosition} from "./getMeshPosition";

export function createItem(
	packedItem: Dimensions & Coordinates,
	index: number,
	material: MeshNormalMaterial,
	binOrigin: Coordinates
) {
	const itemOrigin = getItemOrigin(packedItem);

	const itemGeometry = new BoxGeometry(
		packedItem.length,
		packedItem.height,
		packedItem.width
	);

	const meshPosition = getMeshPosition(binOrigin, itemOrigin, packedItem);
	let mesh = new Mesh(itemGeometry, material);
	mesh.position.set(meshPosition.x, meshPosition.y, meshPosition.z);
	mesh.name = `item_${index}`;
	// mesh.typeName = 'item';
	return mesh;
}
