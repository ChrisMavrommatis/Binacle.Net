import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';
import * as BinacleHelper from 'visualizer/addons/helper.js';

let _visualizerContainer = null;
let _loader = null;
let _rendererContainer = null;
let _logger = {
	isEnabled: true,
	actualLoger: function () { },
	log: function () {
		if (this.isEnabled) {
			this.actualLoger.apply(console, arguments);
		}
	}
};

let _State = {
	aspectRatio: null,
	scene: {},
	camera: {},
	light: {},
	renderer: {},
	controls: {},
}

const windowResizeHandler = function () {
	_State.aspectRatio = window.innerWidth / window.innerHeight;
	_State.camera.aspect = _State.aspectRatio;
	_State.camera.fov = cameraFov(_State.aspectRatio);
	_State.camera.updateProjectionMatrix();
	_loader.style.top = `${loaderTop(_loader)}px`;
	_State.renderer.setSize(_rendererContainer.offsetWidth, rendererHeight());

};
function animate() {
	requestAnimationFrame(animate);
	_State.controls.update();
	render();
}

function render() {
	_State.renderer.render(_State.scene, _State.camera);
}

const rendererHeight = function () {
	//const height = 500 / _State.aspectRatio;
	//return height > 500 ? height : 500;
	return 400;
}

const loaderTop = function (loaderEl) {
	return (rendererHeight() / 2) - (loaderEl.offsetHeight / 2);
}

const cameraFov = function (aspectRatio) {
	if (aspectRatio < 0.60) {
		return 65;
	}
	if (aspectRatio < 1) {
		return 50;
	}
	return 40;
}


window.binacle = {
	state: _State,
	initialize: function (bin) {
		_visualizerContainer = document.getElementById("visualizer-container");
		_loader = _visualizerContainer.querySelector("#loader");
		_rendererContainer = _visualizerContainer.querySelector("#renderer-container");
		_logger.actualLoger = window.console.log;

		_logger.log("Binacle Initialize", bin);
		_State.aspectRatio = window.innerWidth / window.innerHeight;

		_State.scene = new THREE.Scene();
		_State.camera = new THREE.PerspectiveCamera(
			cameraFov(_State.aspectRatio),
			_State.aspectRatio,
			0.1,
			1000
		);
		_State.camera.lookAt(_State.scene.position);
		_State.light = new THREE.AmbientLight(0xffffff);
		_State.light.position.set(0, 0, 0);
		_State.scene.add(_State.light);

		// WebGLRenderer CanvasRenderer
		_State.renderer = new THREE.WebGLRenderer({ antialias: true }); 
		_State.renderer.setClearColor(0xf0f0f0);
		_State.renderer.setPixelRatio(window.devicePixelRatio);
		_State.renderer.setSize(_rendererContainer.offsetWidth, rendererHeight());
		_rendererContainer.append(_State.renderer.domElement);

		_State.controls = new OrbitControls(_State.camera, _State.renderer.domElement);

		_loader.classList.remove("m-6")
		_loader.classList.remove('active');
		_loader.style.position = 'absolute';
		_loader.style.top = `${loaderTop(_loader)}px`;

		window.addEventListener('resize', windowResizeHandler, false);

		animate();

		BinacleHelper.redrawScene(_State.scene, _State.camera, bin, null);

		_logger.log("Binacle Initialized");
	},
	redrawScene: function (bin, packedItems) {
		_logger.log("Binacle Redraw Scene", bin, packedItems);

		BinacleHelper.redrawScene(_State.scene, _State.camera, bin, packedItems);

		_logger.log("Binacle Scene Redrawn");
	},
	invokeErrors: function (errors) {
		_logger.log(errors);
	},
	loadingStart: function () {
		_loader.classList.add('active');
	},
	loadingEnd: function () {
		_loader.classList.remove('active');
	},
	addItemToScene: function (bin, packedItem, index) {
		BinacleHelper.addItemToScene(_State.scene, bin, packedItem, index);
	},
	removeItemFromScene: function (index) {
		BinacleHelper.removeItemFromScene(_State.scene, index);
	}
};

