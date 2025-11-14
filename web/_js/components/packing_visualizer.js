import Control from '../models/control';
import {getThemeColors} from '../utils/theme';
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
	if (bin) {
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
	repeatingInterval: null,
	itemsRendered: 0,
	sceneData: {
		bin: null,
		items: []
	},
	updateScene(resultPromise) {
		this.stopRepeating();
		// loading
		startLoading(_visualizerContainer);
		this.controls.disableAll();

		resultPromise()
			.then(result => {
				if (result.bin && result.packedItems) {
					this.sceneData = {
						bin: result.bin,
						items: result.packedItems
					};

					this.redrawScene(result.bin, result.packedItems);


					this.itemsRendered = this.sceneData.items.length;
				}
				this.controls.updateStatus(this.sceneData, this.itemsRendered);
				stopLoading(_visualizerContainer);
			});
	},
	redrawScene(bin, packedItems) {
		this.$logger.log("[Binacle] Redraw Scene", bin, packedItems);

		redrawScene(_State.scene, _State.camera, bin, packedItems);

		this.$logger.log("[Binacle] Scene Redrawn");
	},
	first() {
		this.controls.disableAll();
		this.redrawScene(this.sceneData.bin, null);
		this.itemsRendered = 0;
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	previous() {
		this.controls.disableAll();
		const index = this.itemsRendered - 1;
		if (index > -1) {
			removeItemFromScene(_State.scene, index);
			this.itemsRendered -= 1;
		}
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	repeat() {
		if (this.repeating) {
			this.stopRepeating();
			return;
		}
		this.repeating = true;
		this.controls.repeat.icon = 'cancel';
		this.controls.repeat.enable();

		this.itemsRendered = 0;
		this.redrawScene(this.sceneData.bin, null);

		let index = 0;
		this.repeatingInterval = setInterval(() => {
			if (!this.repeating || index >= this.sceneData.items.length) {
				this.stopRepeating();
				return;
			}
			const item = this.sceneData.items[index];
			addItemToScene(_State.scene, this.sceneData.bin, item, index);
			this.itemsRendered += 1;
			index += 1;
		}, 1000);
	},
	stopRepeating() {
		if (this.repeatingInterval) {
			clearInterval(this.repeatingInterval);
		}
		this.repeating = false;

		this.controls.repeat.icon = "repeat_one";
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	next() {
		this.controls.disableAll();
		const index = this.itemsRendered;
		if (index < this.sceneData.items.length) {
			const item = this.sceneData.items[index];
			addItemToScene(_State.scene, this.sceneData.bin, item, index);
			this.itemsRendered += 1;
		}
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	last() {
		this.controls.disableAll();
		this.redrawScene(this.sceneData.bin, this.sceneData.items);
		this.itemsRendered = this.sceneData.items.length;
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	init() {
		this.controls = {
			first: new Control(0, "control-first", "first_page", 'bottom-left-round', this.first),
			previous: new Control(1, "control-previous", "chevron_left", 'zero-round', this.previous),
			repeat: new Control(2, "control-repeat", "repeat_one", 'zero-round', this.repeat),
			next: new Control(3, "control-next", "chevron_right", 'zero-round', this.next),
			last: new Control(4, "control-last", "last_page", 'bottom-right-round', this.last),
			all(){
				return [
					this.first,
					this.previous,
					this.repeat,
					this.next,
					this.last
				];
			},
			disableAll() {
				this.all().forEach(control => control.disable());
			},
			enableAll() {
				this.all().forEach(control => control.enable());
			},
			updateStatus(sceneData, itemsRendered) {
				if (!sceneData.bin || !sceneData.items || sceneData.items.length <= 0) {
					this.disableAll();
					return;
				}

				this.repeat.enable();

				if (itemsRendered <= 0) {
					this.first.disable();
					this.previous.disable();
					this.next.enable();
					this.last.enable();
					return;
				}

				if (itemsRendered >= sceneData.items.length) {
					this.first.enable();
					this.previous.enable();
					this.next.disable();
					this.last.disable();
					return;
				}

				this.enableAll();
			}
		};
		this.sceneData.bin = new Bin(60, 40, 10);

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
