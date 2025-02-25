import WebGL from 'three/addons/capabilities/WebGL.js';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';
import {
	cameraFov,
	cameraFar,
	startLoading,
	stopLoading,
	redrawScene,
	addItemToScene,
	removeItemFromScene,
	getThemeColors
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

const getBin = function(scene){
	const bin = scene.getObjectByName('bin').geometry.parameters.geometry.parameters;
	return {
		length : bin.width,
		width: bin.depth,
		height: bin.height
	};
	
}
const windowResizeHandler = function () {
	_State.aspectRatio = window.innerWidth / window.innerHeight;
	_State.camera.aspect = _State.aspectRatio;
	_State.camera.fov = cameraFov(_State.aspectRatio);
	const bin = getBin(_State.scene);
	_State.camera.far = cameraFar(bin);
	_State.camera.updateProjectionMatrix();
	_State.renderer.setSize(
		_rendererContainer.offsetWidth,
		_rendererContainer.offsetHeight
	);
};

const themeChangedHandler = function (event) {
	const themeColors = getThemeColors(window.document.body, "tertiary-container");
	_State.renderer.setClearColor(themeColors.color);
	var bin = _State.scene.getObjectByName('bin');
	if(bin){
		bin.material.color.setHex(themeColors.onColor);
	}
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
		_logger.actualLogger = window.console.log;
		_logger.log("Binacle Initialize", bin);
		
		if (_State.initialized) {
			_logger.log("Binacle already initialized. Reinitializing");
		}
		
		_visualizerContainer = document.getElementById("visualizer-container");
		_rendererContainer = _visualizerContainer.querySelector("#renderer-container");

		_State.aspectRatio = window.innerWidth / window.innerHeight;

		_State.scene = new THREE.Scene();
		_State.camera = new THREE.PerspectiveCamera(
			cameraFov(_State.aspectRatio),
			_State.aspectRatio,
			1,
			cameraFar(bin)
		);
		_State.camera.lookAt(_State.scene.position);
		_State.light = new THREE.AmbientLight(0xffffff);
		_State.light.position.set(0, 0, 0);
		_State.scene.add(_State.light);

		const themeColors = getThemeColors(window.document.body, "tertiary-container");
		// WebGLRenderer CanvasRenderer
		_State.renderer = new THREE.WebGLRenderer({antialias: true});
		_State.renderer.setClearColor(themeColors.color);
		_State.renderer.setPixelRatio(window.devicePixelRatio);
		_State.renderer.setSize(
			_rendererContainer.offsetWidth,
			_rendererContainer.offsetHeight
		);
		_rendererContainer.append(_State.renderer.domElement);

		_State.controls = new OrbitControls(_State.camera, _State.renderer.domElement);

		stopLoading(_visualizerContainer);

		window.addEventListener('resize', windowResizeHandler, false);
		window.addEventListener("themeChanged", themeChangedHandler, false);

		_State.initialized = true;

		animate();

		if (bin) {
			redrawScene(_State.scene, _State.camera, bin, null);
		}

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