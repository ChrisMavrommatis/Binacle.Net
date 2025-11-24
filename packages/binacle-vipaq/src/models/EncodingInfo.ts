import {BitSize} from "./BitSize";
import {Version} from "./Version";


export default class EncodingInfo {
	// 2 Bits
	public version: Version;
	// 2 bits
	public binDimensionsBitSize: BitSize;
	// 2 bits
	public itemDimensionsBitSize: BitSize;
	// 2 bits
	public itemCoordinatesBitSize: BitSize;

	constructor(
		version: Version,
		binDimensionsBitSize: BitSize,
		itemDimensionsBitSize: BitSize,
		itemCoordinatesBitSize: BitSize
	) {
		this.version = version;
		this.binDimensionsBitSize = binDimensionsBitSize;
		this.itemDimensionsBitSize = itemDimensionsBitSize;
		this.itemCoordinatesBitSize = itemCoordinatesBitSize;
	}



}

