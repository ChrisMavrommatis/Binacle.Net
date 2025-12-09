import { Item } from "./item";
import { Bin } from "./bin";

export class DecodedPackingResult {
	public encodedResult: string;
	public bin: Bin;
	public items: Item[];
	constructor(encodedResult:string, bin: Bin, items: Item[]) {
		this.encodedResult = encodedResult;
		this.bin = bin;
		this.items = items;
	}

	binVolume(): number {
		return this.bin.length * this.bin.width * this.bin.height;
	}

	itemsVolume() {
		return this.items.reduce((sum, item) =>
				sum + (item.length * item.width * item.height)
			, 0);
	}

	packedBinVolumePercentage() {
		const packedVolume = this.itemsVolume();
		const binVolume = this.binVolume();
		return Math.round((packedVolume / binVolume) * 100);
	}

}
