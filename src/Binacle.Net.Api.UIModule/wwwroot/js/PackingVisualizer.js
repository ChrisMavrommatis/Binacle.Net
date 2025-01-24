import WebGL from 'three/addons/capabilities/WebGL.js';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';
import {
	cameraFov,
	startLoading,
	stopLoading,
	redrawScene,
	addItemToScene,
	removeItemFromScene
} from "binacle/addons/PackingVisualizer.utils.js";

const _logger = {
	isEnabled: true,
	actualLogger: function () {
	},
	log: function () {
		if (this.isEnabled) {
			this.actualLogger.apply(console, arguments);
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
	initialized: false,
}
let _visualizerContainer = null;
let _rendererContainer = null;

const windowResizeHandler = function () {
	_State.aspectRatio = window.innerWidth / window.innerHeight;
	_State.camera.aspect = _State.aspectRatio;
	_State.camera.fov = cameraFov(_State.aspectRatio);
	_State.camera.updateProjectionMatrix();
	_State.renderer.setSize(
		_rendererContainer.offsetWidth,
		_rendererContainer.offsetHeight
	);

};
function animate() {
	requestAnimationFrame(animate);
	_State.controls.update();
	render();
}

function render() {
	_State.renderer.render(_State.scene, _State.camera);
}

window.binacle = {
	state: _State,
	initialize: function (bin) {
		if (_State.initialized) {
			_logger.log("Binacle already initialized");
			return;
		}
		_visualizerContainer = document.getElementById("visualizer-container");
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
		_State.renderer.setClearColor(0xe7d9f1);
		_State.renderer.setPixelRatio(window.devicePixelRatio);
		_State.renderer.setSize(
			_rendererContainer.offsetWidth,
			_rendererContainer.offsetHeight
		);
		_rendererContainer.append(_State.renderer.domElement);

		_State.controls = new OrbitControls(_State.camera, _State.renderer.domElement);

		stopLoading(_visualizerContainer);

		window.addEventListener('resize', windowResizeHandler, false);

		_State.initialized = true;

		animate();

		redrawScene(_State.scene, _State.camera, bin, null);

		_logger.log("Binacle Initialized");
	},
	redrawScene: function (bin, packedItems) {
		_logger.log("Binacle Redraw Scene", bin, packedItems);

		redrawScene(_State.scene, _State.camera, bin, packedItems);

		_logger.log("Binacle Scene Redrawn");
	},
	invokeErrors: function (errors) {
		_logger.log(errors);
	},
	loadingStart: function () {
		startLoading(_visualizerContainer);
	},
	loadingEnd: function () {
		stopLoading(_visualizerContainer);
	},
	addItemToScene: function (bin, packedItem, index) {
		addItemToScene(_State.scene, bin, packedItem, index);
	},
	removeItemFromScene: function (index) {
		removeItemFromScene(_State.scene, index);
	}
};