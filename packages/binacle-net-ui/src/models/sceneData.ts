import {Coordinates, Dimensions} from "./index";

export default interface SceneData {
	bin: Dimensions | null,
	items: (Dimensions & Coordinates)[]
}

