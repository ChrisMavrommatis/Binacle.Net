import {
	addItemToScene,
	cameraFar,
	cameraFov,
	defineComponent,
	getBin,
	getThemeColors,
	redrawScene,
	removeItemFromScene, startLoading,
	stopLoading
} from "../utils";
import {ControlsManager} from "./index";
import {Control} from "../viewModels";
import {Coordinates, Dimensions, SceneData, VisualizerState} from "../models";
import {AmbientLight, LineBasicMaterial, LineSegments, PerspectiveCamera, Scene, WebGLRenderer} from "three";
import {OrbitControls} from "three/examples/jsm/controls/OrbitControls";
import type { Alpine as AlpineType } from 'alpinejs';


function animate() {
	const state = window.binacle?.visualizerState;
	requestAnimationFrame(animate);
	if (state) {
		state.controls.update();
	}
	render();
};

function render() {
	const state = window.binacle?.visualizerState;
	if (state && state.renderer) {
		state.renderer.render(
			state.scene,
			state.camera
		);
	}
}

function windowResizeHandler() {
	const state = window.binacle?.visualizerState;
	const rendererContainer = window.binacle?.rendererContainer;
	if (!state) {
		return;
	}
	state.aspectRatio = window.innerWidth / window.innerHeight;
	state.camera.aspect = state.aspectRatio;
	state.camera.fov = cameraFov(state.aspectRatio);
	const bin = getBin(state.scene);
	state.camera.far = cameraFar(bin);
	state.camera.updateProjectionMatrix();

	if (rendererContainer) {
		state.renderer.setSize(
			rendererContainer.offsetWidth,
			rendererContainer.offsetHeight
		);
	}
}

function themeChangedHandler(event: Event) {
	const state = window.binacle?.visualizerState;
	if (!state) {
		return;
	}
	const themeColors = getThemeColors(window.document.body, "tertiary-container");
	state.renderer.setClearColor(themeColors.color);
	const bin = state.scene.getObjectByName('bin') as LineSegments;
	if (bin) {
		const colorMat = bin.material as LineBasicMaterial;
		if (colorMat) {
			colorMat.color.setHex(themeColors.onColor);
		}
	}
}


export function packingVisualizerPlugin(Alpine: AlpineType) {
	Alpine.data('packing_visualizer', packingVisualizer);
}

export const packingVisualizer = defineComponent(() => ({
	controls: {} as ControlsManager,
	repeating: false,
	repeatingInterval: null as ReturnType<typeof setInterval> | null,
	itemsRendered: 0,
	sceneData: {
		bin: null,
		items: []
	} as SceneData,
	updateScene(resultPromise: () => Promise<{bin: Dimensions, items: (Dimensions & Coordinates)[]} | null>) {
		this.stopRepeating();
		// loading
		if(window.binacle?.visualizerContainer){
			startLoading(window.binacle.visualizerContainer);
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
				if(window.binacle?.visualizerContainer){
					stopLoading(window.binacle.visualizerContainer);
				}

			})
			.catch((error) => {
				this.$logger.error("[Binacle] Error while updating scene", error);
				this.$dispatch('error-occured', ['Error while updating packing visualizer. Please try again later.']);
				if(window.binacle?.visualizerContainer){
					stopLoading(window.binacle.visualizerContainer);
				}

			});
	},
	redrawScene(bin: Dimensions, items: (Dimensions & Coordinates)[] | null) {
		const state = window.binacle?.visualizerState;
		if (!state) {
			return;
		}
		this.$logger.log("[Binacle] Redraw Scene", bin, items);
		redrawScene(state.scene, state.camera, bin, items);
		this.$logger.log("[Binacle] Scene Redrawn");
	},
	first() {
		this.controls.disableAll();
		this.redrawScene(this.sceneData.bin!, null);
		this.itemsRendered = 0;
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	previous() {
		const state = window.binacle?.visualizerState;
		if (!state) {
			return;
		}
		this.controls.disableAll();
		const index = this.itemsRendered - 1;
		if (index > -1) {
			removeItemFromScene(state.scene, index);
			this.itemsRendered -= 1;
		}
		this.controls.updateStatus(this.sceneData, this.itemsRendered);
	},
	repeat() {
		const state = window.binacle?.visualizerState;
		if(!state) {
			return;
		}
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
			addItemToScene(state.scene, this.sceneData.bin!, item, index);
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
		const state = window.binacle?.visualizerState;
		if(!state) {
			return;
		}
		this.controls.disableAll();
		const index = this.itemsRendered;
		if (index < this.sceneData.items.length) {
			const item = this.sceneData.items[index];
			addItemToScene(state.scene, this.sceneData.bin!, item, index);
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
		this.controls = new ControlsManager(
			new Control(0, "control-first", "first_page", 'bottom-left-round', this.first),
			new Control(1, "control-previous", "chevron_left", 'zero-round', this.previous),
			new Control(2, "control-repeat", "repeat_one", 'zero-round', this.repeat),
			new Control(3, "control-next", "chevron_right", 'zero-round', this.next),
			new Control(4, "control-last", "last_page", 'bottom-right-round', this.last)
		);

		this.$logger.log("[Binacle] Initialize");

		const rendererContainer = this.$refs.renderercontainer;
		const visualizerContainer = this.$refs.visualizercontainer;


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
			rendererContainer.offsetWidth,
			rendererContainer.offsetHeight
		);
		rendererContainer.append(renderer.domElement);

		const controls = new OrbitControls(camera, renderer.domElement);

		stopLoading(visualizerContainer);

		window.addEventListener('resize', windowResizeHandler, false);
		window.addEventListener("themeChanged", themeChangedHandler, false);

		window.binacle = {
			rendererContainer: rendererContainer,
			visualizerContainer: visualizerContainer,
			visualizerState: new VisualizerState(
				aspectRatio,
				scene,
				camera,
				light,
				renderer,
				controls
			)
		};

		animate();

		if (this.sceneData.bin) {
			redrawScene(scene, camera, this.sceneData.bin, null);
		}

		this.$logger.log("[Binacle] Initialized");
	}
}));
