import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';

//const messageContainer = document.getElementById("message-container");
//const warning = WebGL.getWebGL2ErrorMessage();
//messageContainer.appendChild(warning);

const rendererContainer = document.getElementById("renderer-container");
const visualizerContainer = document.getElementById("visualizer-container");

let initialContainer = {
	length: 60,
	width: 40,
	height: 10
}

const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(50, window.innerWidth / window.innerHeight, 0.1, 1000);
camera.lookAt(scene.position);

var light = new THREE.PointLight(0xffffff);
light.position.set(0, 150, 100);
scene.add(light);

const itemMaterial = new THREE.MeshNormalMaterial({ transparent: true, opacity: 0.6 });

const renderer = new THREE.WebGLRenderer({ antialias: true }); // WebGLRenderer CanvasRenderer
renderer.setClearColor(0xf0f0f0);
renderer.setPixelRatio(window.devicePixelRatio);
renderer.setSize(rendererContainer.offsetWidth, 500);
rendererContainer.append(renderer.domElement);

const controls = new OrbitControls(camera, renderer.domElement);
window.addEventListener('resize', onWindowResize, false);

animate();

camera.position.set(60, 60, 60);

var container = createContainer(initialContainer);
scene.add(container);

function onWindowResize() {
	camera.aspect = window.innerWidth / window.innerHeight;
	camera.updateProjectionMatrix();
	renderer.setSize(rendererContainer.offsetWidth, 500);
}
function animate() {
	requestAnimationFrame(animate);
	controls.update();
	render();
}

function render() {
	renderer.render(scene, camera);
}

function createContainer(container) {
	let geometry = new THREE.BoxGeometry(container.length, container.height, container.width);
	let geo = new THREE.EdgesGeometry(geometry); // or WireframeGeometry( geometry )
	let mat = new THREE.LineBasicMaterial({ color: 0x000000, linewidth: 2 });
	let wireframe = new THREE.LineSegments(geo, mat);
	wireframe.position.set(0, 0, 0);
	wireframe.name = 'container';
	return wireframe;
}

window.containerChanged = function (container) {
	var selectedObject = scene.getObjectByName('container');
	scene.remove(selectedObject);
	const newContainer = createContainer(container);
	scene.add(newContainer);
}