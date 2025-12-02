export default class DecodedPackingResult {
	constructor(encodedResult, bin, items) {
		this.encodedResult = encodedResult;
		this.bin = bin;
		this.items = items;
	}
	binVolume() {
		return this.bin.length * this.bin.width * this.bin.height;
	}
	itemsVolume() {
		return this.items.reduce((sum, item) =>
			sum + (item.length * item.width * item.height)
		,0 );
	}
	packedBinVolumePercentage() {
		const packedVolume = this.itemsVolume();
		const binVolume = this.binVolume();
		console.log(packedVolume, binVolume);
		// integer
		return Math.round((packedVolume / binVolume) * 100);


	}

}
