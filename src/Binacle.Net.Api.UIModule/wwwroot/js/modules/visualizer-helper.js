import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';

export const rendererHeight = function () {
	//const height = 500 / _State.aspectRatio;
	//return height > 500 ? height : 500;
	return 400;
}

export const cameraFov = function (aspectRatio) {
	if (aspectRatio < 0.60) {
		return 65;
	}
	if (aspectRatio < 1) {
		return 50;
	}
	return 40;
}

export const loaderTop = function (loaderEl) {
	return (rendererHeight() / 2) - (loaderEl.offsetHeight / 2);
}

function createBin(bin) {
	let geometry = new THREE.BoxGeometry(bin.length, bin.height, bin.width);
	let geo = new THREE.EdgesGeometry(geometry); // or WireframeGeometry( geometry )
	let mat = new THREE.LineBasicMaterial({ color: 0x000000, linewidth: 2 });
	let wireframe = new THREE.LineSegments(geo, mat);
	wireframe.position.set(0, 0, 0);
	wireframe.name = 'bin';
	return wireframe;
}

function createItem(packedItem, index, material, binOrigin) {
	var itemOrigin = {
		x: packedItem.dimensions.length / 2,
		y: packedItem.dimensions.height / 2,
		z: packedItem.dimensions.width / 2
	};

	var itemGeometry = new THREE.BoxGeometry(packedItem.dimensions.length, packedItem.dimensions.height, packedItem.dimensions.width);
	var mesh = new THREE.Mesh(itemGeometry, material);
	mesh.position.set(
		binOrigin.x + itemOrigin.x + packedItem.coordinates.x,
		binOrigin.y + itemOrigin.y + packedItem.coordinates.z,
		binOrigin.z + itemOrigin.z + packedItem.coordinates.y,
	);
	mesh.name = `item_${index}`;
	return mesh;
}

function clearItemsFromScene(state) {
	for (let i = 0; i < state.renderedItems; i++) {
		const selectedObject = state.scene.getObjectByName(`item_${i}`);
		state.scene.remove(selectedObject);
	}
	state.renderedItems = 0;
}

function clearBinFromScene(state) {
	const bin = state.scene.getObjectByName('bin');
	state.scene.remove(bin);
}

export function updateScene(state, loaderEl) {
	loaderEl.classList.add('active');
	clearItemsFromScene(state);
	clearBinFromScene(state);

	state.camera.position.set(state.currentBin.length, state.currentBin.length, state.currentBin.length);

	var bin = createBin(state.currentBin);
	state.scene.add(bin);

	if (state.currentBinResult === null) {
		loaderEl.classList.remove('active');
		return;
	}

	const itemMaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

	const binOrigin = {
		x: -1 * state.currentBin.length / 2,
		y: -1 * state.currentBin.height / 2,
		z: -1 * state.currentBin.width / 2
	}

	for (var i = 0; i < state.currentBinResult.packedItems.length; i++) {

		var item = createItem(state.currentBinResult.packedItems[i], i, itemMaterial, binOrigin);
		state.renderedItems += i;
		state.scene.add(item);
	}
	loaderEl.classList.remove('active');
}