import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';
import * as BinacleHelper from '/js/modules/visualizer-helper.js';

let _visualizerContainer = null;
let _loader = null;
let _rendererContainer = null;
let _logger = null;
let _State = {
	aspectRatio: null,
	currentBin: null,
	currentBinResult: null,
	bins: [],
	binResults: [],
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
	_State.camera.fov = BinacleHelper.cameraFov(_State.aspectRatio);
	_State.camera.updateProjectionMatrix();
	_loader.style.top = `${BinacleHelper.loaderTop(_loader)}px`;
	//_loader.style.left = `${loaderLeft()}px`;
	_State.renderer.setSize(_rendererContainer.offsetWidth, BinacleHelper.rendererHeight());

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
		_State.camera = new THREE.PerspectiveCamera(BinacleHelper.cameraFov(_State.aspectRatio), _State.aspectRatio, 0.1, 1000);
		_State.camera.lookAt(_State.scene.position);
		_State.light = new THREE.PointLight(0xffffff);
		_State.light.position.set(0, 150, 100);
		_State.scene.add(_State.light);

		_State.renderer = new THREE.WebGLRenderer({ antialias: true }); // WebGLRenderer CanvasRenderer
		_State.renderer.setClearColor(0xf0f0f0);
		_State.renderer.setPixelRatio(window.devicePixelRatio);
		_State.renderer.setSize(_rendererContainer.offsetWidth, BinacleHelper.rendererHeight());
		_rendererContainer.append(_State.renderer.domElement);

		_State.controls = new OrbitControls(_State.camera, _State.renderer.domElement);

		_loader.classList.remove("m-6")
		_loader.classList.remove('active');
		_loader.style.position = 'absolute';
		_loader.style.top = `${BinacleHelper.loaderTop(_loader)}px`;
		//_loader.style.left = `${loaderLeft()}px`;

		window.addEventListener('resize', windowResizeHandler, false);

		animate();

		BinacleHelper.updateScene(_State, _loader);

		_logger.log("Binacle Initialized", _State.currentBin);
	},
	binsChanged: function (bins) {
		_loader.classList.add('active');

		_State.bins = bins;
		_State.currentBin = bins[0];
		_State.binResults = [];
		_State.currentBinResult = null;
		BinacleHelper.updateScene(_State, _loader);
	},
	updateResults: function (results) {
		_loader.classList.add('active');

		_logger.log("Binacle Updating Results", results);
		_State.binResults = results.data;
		var currentBinResults = results.data.filter(x => x.bin.id === _State.currentBin.id);
		_State.currentBinResult = currentBinResults.length === 1 ? currentBinResults[0] : null;

		BinacleHelper.updateScene(_State, _loader);

		_logger.log("Binacle Updated Results");
	},

	invokeErrors: function (errors) {
		_logger.log(errors);
	},
	loading: function () {
		_loader.classList.add('active');
	}
};
