import Control from '../models/control';
import { getThemeColors } from '../utils/theme';
import Bin from '../models/bin';
import {
	cameraFov,
	cameraFar,
	startLoading,
	stopLoading,
	redrawScene,
	addItemToScene,
	removeItemFromScene,
	getBin,
} from '../utils/visualizer';

import {OrbitControls} from 'three/addons/controls/OrbitControls.js';
import {
	Scene,
	PerspectiveCamera,
	AmbientLight,
	WebGLRenderer
} from 'three';

const _State = {
	aspectRatio: null,
	scene: null,
	camera: null,
	light: null,
	renderer: null,
	controls: null,
	initialized: false,
};
let _rendererContainer = null;
let _visualizerContainer = null;

function windowResizeHandler() {
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

function themeChangedHandler(event) {
	const themeColors = getThemeColors(window.document.body, "tertiary-container");
	_State.renderer.setClearColor(themeColors.color);
	const bin = _State.scene.getObjectByName('bin');
	if(bin){
		bin.material.color.setHex(themeColors.onColor);
	}

};
function animate() {
	requestAnimationFrame(animate);
	_State.controls.update();
	render();
};
function render() {
	_State.renderer.render(
		_State.scene,
		_State.camera
	);
}


export default () => ({
	controls: {},
	repeating: false,
	itemsRendered: 0,
	sceneData: {
		bin: null,
		items: []
	},
	updateScene(resultPromise){
		this.stopRepeating();
		// loading
		this.disableAllControls();

		resultPromise()
			.then(result => {
				if(result.bin && result.packedItems){
					this.sceneData = {
						bin: result.bin,
						items: result.packedItems
					};

					this.$logger.log("[Binacle] Redraw Scene", result.bin, result.packedItems);

					redrawScene(_State.scene, _State.camera, result.bin, result.packedItems);

					this.$logger.log("[Binacle] Scene Redrawn");

					this.itemsRendered = this.sceneData.items.length;
				}
				this.updateControlsStatus();
			});



	},
	first() {

	},
	previous() {

	},
	repeat() {

	},
	next() {

	},
	last() {

	},
	disableAllControls(){
		for (const key in this.controls)
		{
			this.controls[key].enabled = false;
		}

	},
	updateControlsStatus()
	{
		if (!this.sceneData.bin || !this.sceneData.items || this.sceneData.items.length <= 0)
		{
			this.disableAllControls();
			return;
		}

		this.controls.repeat.enabled = true;

		if (this.itemsRendered <= 0)
		{
			this.controls.first.enabled = false;
			this.controls.previous.enabled = false;
			this.controls.next.enabled = true;
			this.controls.last.enabled = true;
			return;
		}

		if (this.itemsRendered >= this.sceneData.items.length)
		{
			this.controls.first.enabled = true;
			this.controls.previous.enabled = true;
			this.controls.next.enabled = false;
			this.controls.last.enabled = false;
			return;
		}

		this.controls.first.enabled = true;
		this.controls.previous.enabled = true;
		this.controls.repeat.enabled = true;
		this.controls.next.enabled = true;
		this.controls.last.enabled = true;
	},
	stopRepeating(){
		this.repeating = false;
		this.controls.repeat.icon = "repeat_one";
	},
	init() {
		this.controls = {
			first: new Control(0, "control-first", "first_page", 'bottom-left-round', this.first),
			previous: new Control(1, "control-previous", "chevron_left", 'zero-round', this.previous),
			repeat: new Control(2, "control-repeat", "repeat_one", 'zero-round', this.repeat),
			next: new Control(3, "control-next", "chevron_right", 'zero-round', this.next),
			last: new Control(4, "control-last", "last_page", 'bottom-right-round', this.last),
		};
		this.sceneData.bin = new Bin(60,40,30);

		this.$logger.log("[Binacle] Initialize");

		_rendererContainer = this.$refs.renderercontainer;
		_visualizerContainer = this.$refs.visualizercontainer;

		window.binacle = window.binacle || {};
		window.binacle.visualizerState = _State;
		window.binacle.rendererContainer = _rendererContainer;
		window.binacle.visualizerContainer = _visualizerContainer;



		_State.aspectRatio = window.innerWidth / window.innerHeight;
		_State.scene = new Scene();
		_State.camera = new PerspectiveCamera(
			cameraFov(_State.aspectRatio),
			_State.aspectRatio,
			1,
			cameraFar(this.sceneData.bin)
		);

		_State.camera.lookAt(_State.scene.position);
		_State.light = new AmbientLight(0xffffff);
		_State.light.position.set(0, 0, 0);
		_State.scene.add(_State.light);

		const themeColors = getThemeColors(window.document.body, "tertiary-container");

		// WebGLRenderer CanvasRenderer
		_State.renderer = new WebGLRenderer({antialias: true});
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

		if (this.sceneData.bin) {
			redrawScene(_State.scene, _State.camera, this.sceneData.bin, null);
		}

		this.$logger.log("[Binacle] Initialized");
	}

});
