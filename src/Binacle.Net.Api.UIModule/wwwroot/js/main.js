import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';

//const messageContainer = document.getElementById("message-container");
//const warning = WebGL.getWebGL2ErrorMessage();
//messageContainer.appendChild(warning);


let _visualizerContainer = null;
let _loader = null;
let _rendererContainer = null;
let _logger = null;
let _State = {
	aspectRatio: null,
	currentBin: {},
	currentItems: [],
	bins: [],
	items: [],
	scene: {},
	camera: {},
	light: {},
	renderer: {},
	controls: {},
	renderedItems: 0
}

const windowResizeHandler = function () {
	_State.aspectRatio = window.innerWidth / window.innerHeight;
	_State.camera.aspect = _State.aspectRatio;
	_State.camera.updateProjectionMatrix();

	const height = 500 / _State.aspectRatio;
	_State.renderer.setSize(_rendererContainer.offsetWidth, height > 500 ? height : 500);

};
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

function animate() {
	requestAnimationFrame(animate);
	_State.controls.update();
	render();
}

function render() {
	_State.renderer.render(_State.scene, _State.camera);
}

function clearItems() {
	for (let i = 0; i < _State.renderedItems; i++) {
		const selectedObject = _State.scene.getObjectByName(`item_${i}`);
		_State.scene.remove(selectedObject);
	}
	_State.renderedItems = 0;
}

window.binacle = {
	initialize: function (bins) {
		_visualizerContainer = document.getElementById("visualizer-container");
		_loader = _visualizerContainer.querySelector("#loader");
		_rendererContainer = _visualizerContainer.querySelector("#renderer-container");
		_logger = window.console;


		_logger.log("Binacle Initialize", bins);
		_State.aspectRatio = window.innerWidth / window.innerHeight;
		_State.bins = bins;
		_State.currentBin = bins[0];

		_State.scene = new THREE.Scene();
		_State.camera = new THREE.PerspectiveCamera(50, _State.aspectRatio, 0.1, 1000);
		_State.camera.lookAt(_State.scene.position);
		_State.light = new THREE.PointLight(0xffffff);
		_State.light.position.set(0, 150, 100);
		_State.scene.add(_State.light);

		_State.renderer = new THREE.WebGLRenderer({ antialias: true }); // WebGLRenderer CanvasRenderer
		_State.renderer.setClearColor(0xf0f0f0);
		_State.renderer.setPixelRatio(window.devicePixelRatio);
		const height = 500 / _State.aspectRatio;
		_State.renderer.setSize(_rendererContainer.offsetWidth, height > 500 ? height : 500);
		_rendererContainer.append(_State.renderer.domElement);

		_State.controls = new OrbitControls(_State.camera, _State.renderer.domElement);
		_State.controls.enable = true;
		_State.controls.enableRotate = true;
		_loader.classList.remove('active');
		_loader.style.display = 'none';
		window.addEventListener('resize', windowResizeHandler, false);

		animate();

		_State.camera.position.set(_State.currentBin.length, _State.currentBin.length, _State.currentBin.length);

		var bin = createBin(_State.currentBin);
		_State.scene.add(bin);


		clearItems();

		_logger.log("Binacle Initialized", _State.currentBin);
	},
	updateResults: function (results) {
		_logger.log("Binacle Updating Results", results);
		clearItems();

		const itemMaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

		const binOrigin = {
			x: -1 * _State.currentBin.length / 2,
			y: -1 * _State.currentBin.height / 2,
			z: -1 * _State.currentBin.width / 2
		}
		var data = results.data[0];

		for (var i = 0; i < data.packedItems.length; i++) {

			var item = createItem(data.packedItems[i], i, itemMaterial, binOrigin);
			_State.renderedItems += i;
			_State.scene.add(item);
			_logger.log("Binacle Updated Results");
		}

	},
	invokeErrors: function (errors) {
		_logger.log(errors);
	}
};
