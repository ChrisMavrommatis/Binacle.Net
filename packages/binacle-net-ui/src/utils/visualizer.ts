import {
	BoxGeometry,
	EdgesGeometry,
	LineBasicMaterial,
	LineSegments,
	Mesh,
	MeshNormalMaterial,
	PerspectiveCamera,
	Scene
} from 'three';

import {getThemeColors} from './theme';
import {Bin} from "../models/bin";
import {Item} from "../models/item";
import {Coordinates} from "../models/coordinates";
import {PackedItem} from "../models/packedItem";
import {Dimensions} from "../models/dimensions";


const _itemMaterial = new MeshNormalMaterial({transparent: true, opacity: 0.6});

export function cameraFov(aspectRatio: number) {
	if (aspectRatio < 0.60) {
		return 65;
	}
	if (aspectRatio < 1) {
		return 50;
	}
	return 40;
}

export function cameraFar(bin: Bin | null) {
	if (!bin) {
		return 1000;
	}
	const distance = Math.sqrt((bin.length ** 2) + (bin.height ** 2) + (bin.width ** 2));
	const far = (distance * 2) + (bin.height * 2);
	return Math.ceil(far);
}


export function startLoading(container: HTMLElement) {
	const loader = document.createElement("progress");
	loader.id = "loader";
	loader.classList.add("circle", "large", "absolute", "center", "middle");
	container.append(loader);
}

export function stopLoading(container: HTMLElement) {
	const loader = container.querySelector("#loader");
	if (loader) {
		loader.remove();
	}
}

export function getCameraPosition(bin: Bin) {
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
	} as Coordinates
}

export function getBin(scene: Scene) {
	const binObject = scene.getObjectByName('bin') as LineSegments;
	if (!binObject) {
		return null;
	}
	const binGeometry = binObject.geometry as EdgesGeometry;
	const boxGeometry = binGeometry.parameters.geometry as BoxGeometry;
	const bin = boxGeometry.parameters;
	return {
		length: bin.width,
		width: bin.depth,
		height: bin.height
	} as Bin;
}

export function getBinOrigin(bin: Dimensions) {
	return {
		x: -1 * bin.length / 2,
		y: -1 * bin.height / 2,
		z: -1 * bin.width / 2
	} as Coordinates;
}

export function getItemOrigin(item: Dimensions) {
	return {
		x: item.length / 2,
		y: item.height / 2,
		z: item.width / 2
	} as Coordinates;
}

export function getMeshPosition(binOrigin: Coordinates, itemOrigin: Coordinates, packedItem: Coordinates) {
	return {
		x: binOrigin.x + itemOrigin.x + packedItem.x,
		y: binOrigin.y + itemOrigin.y + packedItem.z,
		z: binOrigin.z + itemOrigin.z + packedItem.y,
	};
}

export function clearItemsFromScene(scene: Scene) {
	const items = scene.getObjectsByProperty("typeName", "item");
	for (let i = 0; i < items.length; i++) {
		scene.remove(items[i]);
	}
}

export function clearBinFromScene(scene: Scene) {
	const bin = scene.getObjectByName('bin');
	if (bin) {
		scene.remove(bin);
	}
}

export function addItemToScene(scene: Scene, bin: Bin, item: PackedItem, index: number) {
	const binOrigin = getBinOrigin(bin);
	let createdItem = createItem(item, index, _itemMaterial, binOrigin);
	scene.add(createdItem);
}

export function removeItemFromScene(scene: Scene, index: number) {
	const item = scene.getObjectByName(`item_${index}`);
	if (item) {
		scene.remove(item);
	}
}

export function redrawScene(scene: Scene, camera: PerspectiveCamera, bin: Bin, items: PackedItem[] | null) {
	clearItemsFromScene(scene);
	clearBinFromScene(scene);

	const cameraPosition = getCameraPosition(bin);
	camera.position.set(cameraPosition.x, cameraPosition.y, cameraPosition.z);

	camera.far = cameraFar(bin);
	const bin3D = createBin(bin);
	scene.add(bin3D);

	if (!items || items.length < 1)
	{
		return;
	}

	const binOrigin = getBinOrigin(bin);

	for (let i = 0; i < items.length; i++ )
	{
		let item = createItem(items[i], i, _itemMaterial, binOrigin);
		scene.add(item);
	}

	camera.updateProjectionMatrix();
}

function createItem(item: PackedItem, index: number, material:any, binOrigin: Coordinates) {
	const itemOrigin = getItemOrigin(item);

	const itemGeometry = new BoxGeometry(
		item.length,
		item.height,
		item.width
	);

	const meshPosition = getMeshPosition(binOrigin, itemOrigin, item);

	let mesh = new Mesh(itemGeometry, material);
	mesh.position.set(meshPosition.x, meshPosition.y, meshPosition.z);
	mesh.name = `item_${index}`;
	mesh.typeName = 'item';
	return mesh;
}

function createBin(bin: Bin) {
	let geometry = new BoxGeometry(bin.length, bin.height, bin.width);

	// or WireframeGeometry( geometry )
	let geo = new EdgesGeometry(geometry);

	const colors = getThemeColors(window.document.body, "tertiary-container");
	let mat = new LineBasicMaterial({color: colors.onColor, linewidth: 4});
	let wireframe = new LineSegments(geo, mat);
	wireframe.position.set(0, 0, 0);
	wireframe.name = 'bin';
	return wireframe;
}
