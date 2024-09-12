import WebGL from 'three/addons/capabilities/WebGL.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
import * as THREE from 'three';

//const messageContainer = document.getElementById("message-container");
//const warning = WebGL.getWebGL2ErrorMessage();
//messageContainer.appendChild(warning);

const rendererContainer = document.getElementById("renderer-container");
const visualizerContainer = document.getElementById("visualizer-container");

let _Container = {
	length: 60,
	width: 40,
	height: 10
}
let _ContainerOriginOffset = {
	x: -1 * _Container.length / 2,
	y: -1 * _Container.height / 2,
	z: -1 * _Container.width / 2
}
let _Results = {};

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

var container = createContainer(_Container);
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
	_Container = createContainer(container);
	scene.add(_Container);
}
window.updateResults = function (rawResults) {
	_Results = JSON.parse(rawResults);
	var data = _Results.data[0];

	for (var i = 0; i < data.packedItems.length; i++) {

		var item = data.packedItems[i];
		var itemOriginOffset = {
			x: item.dimensions.length / 2,
			y: item.dimensions.height / 2,
			z: item.dimensions.width / 2
		};

		var itemGeometry = new THREE.BoxGeometry(item.dimensions.length, item.dimensions.height, item.dimensions.width);
		var cube = new THREE.Mesh(itemGeometry, itemMaterial);
		cube.position.set(
			_ContainerOriginOffset.x + itemOriginOffset.x + item.coordinates.x,
			_ContainerOriginOffset.y + itemOriginOffset.y + item.coordinates.z,
			_ContainerOriginOffset.z + itemOriginOffset.z + item.coordinates.y,
		);
		cube.name = 'cube' + i;
		scene.add(cube);
	}
	// Remove old items


}