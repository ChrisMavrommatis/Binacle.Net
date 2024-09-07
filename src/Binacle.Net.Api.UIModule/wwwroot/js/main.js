import WebGL from 'three/addons/capabilities/WebGL.js';

if (WebGL.isWebGL2Available()) {

	// Initiate function or other initializations here
	console.log("not")

} else {

	console.log("Yes")

}

const warning = WebGL.getWebGL2ErrorMessage();
console.log(warning);