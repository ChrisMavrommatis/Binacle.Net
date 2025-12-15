import {Light, PerspectiveCamera, Scene, WebGLRenderer } from "three";
import {OrbitControls} from "three/examples/jsm/controls/OrbitControls";

export default class VisualizerState {
	public aspectRatio: number;
	public scene: Scene;
	public camera: PerspectiveCamera;
	public light: Light;
	public renderer: WebGLRenderer;
	public controls: OrbitControls;
	public initialized: boolean;

	constructor(
		aspectRatio: number,
		scene: Scene,
		camera: PerspectiveCamera,
		light: Light,
		renderer: WebGLRenderer,
		controls: OrbitControls,
	) {
		this.aspectRatio = aspectRatio;
		this.scene = scene;
		this.camera = camera;
		this.light = light;
		this.renderer = renderer;
		this.controls = controls;
		this.initialized = false;
	}
};
