import Coordinates from "./coordinates";
import Dimensions from "./dimensions";

export default class Item implements Dimensions, Coordinates{
	public length!: number;
	public width!: number;
	public height!: number;
	public x!: number;
	public y!: number;
	public z!: number;
}

