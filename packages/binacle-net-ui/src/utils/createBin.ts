import { BoxGeometry, EdgesGeometry, LineBasicMaterial, LineSegments } from "three";
import {Dimensions} from "../models";
import {getThemeColors} from "./getThemeColors";

export function createBin(bin: Dimensions) {
	let geometry = new BoxGeometry(bin.length, bin.height, bin.width);

	// or WireframeGeometry( geometry )
	let geo = new EdgesGeometry(geometry);

	const colors = getThemeColors(window.document.body, "tertiary-container");
	let mat = new LineBasicMaterial({color: colors.onColor, linewidth: 4});
	let wireframe = new LineSegments(geo, mat);
	wireframe.position.set(0, 0, 0);
	wireframe.name = 'bin';
	return wireframe;
}
