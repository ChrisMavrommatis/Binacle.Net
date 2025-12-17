import {Coordinates, Dimensions} from "binacle-vipaq";

export default class DecodedPackingResult {

	public encodedResult: string;
	public bin: Dimensions
	public items: (Dimensions & Coordinates)[]

	constructor(
		encodedResult: string,
		bin: Dimensions,
		items: (Dimensions & Coordinates)[]
	){
		this.encodedResult = encodedResult;
		this.bin = bin;
		this.items = items;
	}

	binVolume(){
		return this.bin.length * this.bin.width * this.bin.height;
	}
	itemsVolume() {
		return this.items.reduce((sum, item) => {
			return sum + (item.length * item.width * item.height);
		}, 0);
	}
	packedBinVolumePercentage(){
		const packedVolume = this.itemsVolume();
		const binVolume = this.binVolume();
		return Math.round((packedVolume / binVolume) * 100);
	}
}
