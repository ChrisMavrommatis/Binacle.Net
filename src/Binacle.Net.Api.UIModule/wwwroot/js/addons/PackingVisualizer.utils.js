import * as THREE from 'three';

const _itemMaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

export function cameraFov(aspectRatio) {
	if (aspectRatio < 0.60) {
		return 65;
	}
	if (aspectRatio < 1) {
		return 50;
	}
	return 40;
}

export function startLoading(container) {
	const loader = document.createElement("progress");
	loader.id = "loader";
	loader.classList.add("circle", "large", "absolute", "center", "middle");
	container.append(loader);
}

export function stopLoading(container) {
	const loader = container.querySelector("#loader");
	loader.remove();
}

export function getCameraPosition(bin) {
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

export function getBinOrigin(bin) {
	return {
		x: -1 * bin.length / 2,
		y: -1 * bin.height / 2,
		z: -1 * bin.width / 2
	};
}

export function getItemOrigin(packedItem) {
	return {
		x: packedItem.dimensions.length / 2,
		y: packedItem.dimensions.height / 2,
		z: packedItem.dimensions.width / 2
	};
}

export function getMeshPosition(binOrigin, itemOrigin, packedItem) {
	return {
		x: binOrigin.x + itemOrigin.x + packedItem.coordinates.x,
		y: binOrigin.y + itemOrigin.y + packedItem.coordinates.z,
		z: binOrigin.z + itemOrigin.z + packedItem.coordinates.y,
	};
}

export function clearItemsFromScene(scene) {
	const items = scene.getObjectsByProperty("typeName", "item");
	for (let i = 0; i < items.length; i++) {
		scene.remove(items[i]);
	}
}

export function clearBinFromScene(scene) {
	const bin = scene.getObjectByName('bin');
	scene.remove(bin);
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

export function redrawScene(scene, camera, bin, packedItems) {
	clearItemsFromScene(scene);
	clearBinFromScene(scene);

	const cameraPosition = getCameraPosition(bin);
	camera.position.set(cameraPosition.x, cameraPosition.y, cameraPosition.z);

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
}

function createItem(packedItem, index, material, binOrigin) {
	const itemOrigin = getItemOrigin(packedItem);

	const itemGeometry = new THREE.BoxGeometry(
		packedItem.dimensions.length,
		packedItem.dimensions.height,
		packedItem.dimensions.width
	);

	const meshPosition = getMeshPosition(binOrigin, itemOrigin, packedItem);

	let mesh = new THREE.Mesh(itemGeometry, material);
	mesh.position.set(meshPosition.x, meshPosition.y, meshPosition.z);
	mesh.name = `item_${index}`;
	mesh.typeName = 'item';
	return mesh;
}

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