import * as THREE from 'three';

const timer = ms => new Promise(res => setTimeout(res, ms));

const _itemMaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

function createBin(bin) {
	let geometry = new THREE.BoxGeometry(bin.length, bin.height, bin.width);

	// or WireframeGeometry( geometry )
	let geo = new THREE.EdgesGeometry(geometry);

	let mat = new THREE.LineBasicMaterial({ color: 0x000000, linewidth: 4 });
	let wireframe = new THREE.LineSegments(geo, mat);
	wireframe.position.set(0, 0, 0);
	wireframe.name = 'bin';
	return wireframe;
}

function createItem(packedItem, index, material, binOrigin) {
	let itemOrigin = {
		x: packedItem.dimensions.length / 2,
		y: packedItem.dimensions.height / 2,
		z: packedItem.dimensions.width / 2
	};

	let itemGeometry = new THREE.BoxGeometry(
		packedItem.dimensions.length,
		packedItem.dimensions.height,
		packedItem.dimensions.width
	);

	let mesh = new THREE.Mesh(itemGeometry, material);
	mesh.position.set(
		binOrigin.x + itemOrigin.x + packedItem.coordinates.x,
		binOrigin.y + itemOrigin.y + packedItem.coordinates.z,
		binOrigin.z + itemOrigin.z + packedItem.coordinates.y,
	);
	mesh.name = `item_${index}`;
	mesh.typeName = 'item';
	return mesh;
}

function clearItemsFromScene(scene) {
	var items = scene.getObjectsByProperty("typeName", "item");
	for (var i = 0; i < items.length; i++) {
		scene.remove(items[i]);
	}
}

function clearBinFromScene(scene) {
	const bin = scene.getObjectByName('bin');
	scene.remove(bin);
}

function getCameraPosition(bin) {

	var longestDimension = bin.length;

	if (bin.height > longestDimension) {
		longestDimension = bin.height;
	}
	if (bin.width > longestDimension) {
		longestDimension = bin.width;
	}

	var cameraPositionX = longestDimension;
	var cameraPositionY = longestDimension;
	var cameraPositionZ = longestDimension * 1.2;

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

function getBinOrigin(bin) {
	return {
		x: -1 * bin.length / 2,
		y: -1 * bin.height / 2,
		z: -1 * bin.width / 2
	};
}


export function redrawScene(scene, camera, bin, packedItems) {
	clearItemsFromScene(scene);
	clearBinFromScene(scene);

	var cameraPosition = getCameraPosition(bin);
	camera.position.set(cameraPosition.x, cameraPosition.y, cameraPosition.z);

	var bin3D = createBin(bin);
	scene.add(bin3D);

	if (!packedItems || packedItems.length < 1) {
		return;
	}

	const binOrigin = getBinOrigin(bin);

	for (var i = 0; i < packedItems.length; i++) {

		let item = createItem(packedItems[i], i, _itemMaterial, binOrigin);

		scene.add(item);
	}
}

export function addItemToScene(scene, bin, packedItem, index) {
	const binOrigin = getBinOrigin(bin);

	let item = createItem(packedItem, index, _itemMaterial, binOrigin);

	scene.add(item);
}

export function removeItemFromScene(scene, index) {
	const item = scene.getObjectByName(`item_${index}`);
	scene.remove(item);
}