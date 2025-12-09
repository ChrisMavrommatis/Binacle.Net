import { Control } from '../models/control';
import { Bin } from '../models';
import {
	Scene,
	PerspectiveCamera,
	AmbientLight,
	WebGLRenderer,
	LineSegments,
	LineBasicMaterial
} from 'three';
import { VisualizerState } from '@/models';
import { ControlsManager } from '../models/controlsManager';
import { SceneData } from '../models/sceneData';
import { defineComponent } from '../utils/defineComponent';
import {
	addItemToScene,
	cameraFar,
	cameraFov,
	getBin,
	redrawScene,
	removeItemFromScene,
	startLoading,
	stopLoading
} from '../utils/visualizer';
import { getThemeColors } from '../utils/theme';
import { PackedItem } from '../models/packedItem';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls';

let _State: VisualizerState | null = null;
let _RendererContainer: HTMLElement | null = null;
let _VisualizerContainer: HTMLElement | null = null;

function windowResizeHandler() {
	if (!_State) {
		return;
	}
	_State.aspectRatio = window.innerWidth / window.innerHeight;
	_State.camera.aspect = _State.aspectRatio;
	_State.camera.fov = cameraFov(_State.aspectRatio);
	const bin = getBin(_State.scene);
	_State.camera.far = cameraFar(bin);
	_State.camera.updateProjectionMatrix();
	if (_RendererContainer) {
		_State.renderer.setSize(
			_RendererContainer.offsetWidth,
			_RendererContainer.offsetHeight
		);
	}
}

function themeChangedHandler(event: Event) {
	if (!_State || !_State.renderer || !_State.scene) {
		return;
	}
	const themeColors = getThemeColors(window.document.body, "tertiary-container");
	_State.renderer.setClearColor(themeColors.color);
	const bin = _State.scene.getObjectByName('bin') as LineSegments;
	if (bin) {
		const colorMat = bin.material as LineBasicMaterial;
		if (colorMat) {
			colorMat.color.setHex(themeColors.onColor);
		}
	}

}

function animate() {
	requestAnimationFrame(animate);
	if (_State) {
		_State.controls.update();
	}
	render();
}

function render() {
	if (!_State) {
		return;
	}
	_State.renderer.render(
		_State.scene,
		_State.camera
	);
}


export default defineComponent(() => ({
	controls: {} as ControlsManager,
	repeating: false,
	repeatingInterval: null as ReturnType<typeof setInterval> | null,
	itemsRendered: 0,
	sceneData: {
		bin: null,
		items: []
	} as SceneData,
	updateScene(resultPromise: () => Promise<SceneData>) {
		this.stopRepeating();
		// loading
		if (_VisualizerContainer) {
			startLoading(_VisualizerContainer);
		}
		this.controls.disableAll();

		resultPromise()
			.then(result => {
				if (result && result.bin && result.items) {
					this.sceneData = {
						bin: result.bin,
						items: result.items
					};
					this.redrawScene(result.bin, result.items);
					this.itemsRendered = this.sceneData.items.length;
				}
				this.controls.updateStatus(this.sceneData, this.itemsRendered);
				if (_VisualizerContainer) {
					stopLoading(_VisualizerContainer);
				}
			})
			.catch((error) => {
				this.$logger.error("[Binacle] Error while updating scene", error);
				this.$dispatch('error-occured', ['Error while updating packing visualizer. Please try again later.']);
				if (_VisualizerContainer) {
					stopLoading(_VisualizerContainer);
				}
			});
	},
	redrawScene(bin: Bin, items: PackedItem[] | null) {
		if (!_State) {
			return;
		}
		// this.$logger.log("[Binacle] Redraw Scene", bin, items);
		redrawScene(_State.scene, _State.camera, bin, items);

		// this.$logger.log("[Binacle] Scene Redrawn");
	},
	first() {
		this.controls.disableAll();
		this.redrawScene(this.sceneData.bin!, null);
		this.itemsRendered = 0;
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	previous() {
		this.controls.disableAll();
		const index = this.itemsRendered - 1;
		if (index > -1) {
			removeItemFromScene(_State!.scene, index);
			this.itemsRendered -= 1;
		}
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	repeat() {
		this.controls.disableAll();

		if (this.repeating) {
			this.stopRepeating();
			return;
		}
		this.repeating = true;
		this.controls.repeat.icon = 'cancel';
		this.controls.repeat.enable();

		this.itemsRendered = 0;
		this.redrawScene(this.sceneData.bin!, null);

		let index = 0;
		this.repeatingInterval = setInterval(() => {
			if (!this.repeating || index >= this.sceneData.items.length) {
				this.stopRepeating();
				return;
			}
			const item = this.sceneData.items[index];
			addItemToScene(_State!.scene, this.sceneData.bin!, item, index);
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
			addItemToScene(_State!.scene, this.sceneData.bin!, item, index);
			this.itemsRendered += 1;
		}
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	last() {
		this.controls.disableAll();
		this.redrawScene(this.sceneData.bin!, this.sceneData.items);
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
			all() {
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

		this.$logger.log("[Binacle] Initialize");

		_RendererContainer = this.$refs.renderercontainer;
		_VisualizerContainer = this.$refs.visualizercontainer;

		window.binacle.visualizerState = _State;
		window.binacle.rendererContainer = _RendererContainer;
		window.binacle.visualizerContainer = _VisualizerContainer;


		const aspectRatio = window.innerWidth / window.innerHeight;
		const scene = new Scene();
		const camera = new PerspectiveCamera(
			cameraFov(aspectRatio),
			aspectRatio,
			1,
			cameraFar(this.sceneData.bin)
		);

		camera.lookAt(scene.position);
		const light = new AmbientLight(0xffffff);
		light.position.set(0, 0, 0);
		scene.add(light);

		const themeColors = getThemeColors(window.document.body, "tertiary-container");

		// WebGLRenderer CanvasRenderer
		const renderer = new WebGLRenderer({antialias: true});
		renderer.setClearColor(themeColors.color);
		renderer.setPixelRatio(window.devicePixelRatio);
		renderer.setSize(
			_RendererContainer.offsetWidth,
			_RendererContainer.offsetHeight
		);
		_RendererContainer.append(renderer.domElement);

		const controls = new OrbitControls(camera, renderer.domElement);

		stopLoading(_VisualizerContainer);

		window.addEventListener('resize', windowResizeHandler, false);
		window.addEventListener("themeChanged", themeChangedHandler, false);

		_State = new VisualizerState(
			aspectRatio,
			scene,
			camera,
			light,
			renderer,
			controls
		);
		_State.initialized = true;

		animate();

		if (this.sceneData.bin) {
			redrawScene(_State.scene, _State.camera, this.sceneData.bin, null);
		}

		this.$logger.log("[Binacle] Initialized");
	}
}));
