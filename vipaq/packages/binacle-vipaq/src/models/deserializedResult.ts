import Dimensions from "./dimensions";
import Coordinates from "./coordinates";

export default class DeserializedResult{
	public bin: Dimensions;
	public items: (Dimensions & Coordinates)[];

	constructor(bin: Dimensions, items: (Dimensions & Coordinates)[]) {
		this.bin = bin;
		this.items = items;
	}
}
