import {BitSize, Coordinates, Dimensions} from "./models";
import {Sizes} from "./utils";

export class ProtocolWriter {
	private offset: number;
	public buffer: Uint8Array<ArrayBuffer>;
	private data: DataView<ArrayBuffer>;

	constructor(bufferSize: number) {
		this.buffer = new Uint8Array(bufferSize);
		this.data = new DataView(this.buffer.buffer);
		this.offset = 0;
	}

	// Range-check before every write, like C#'s CreateChecked. A value that does not fit the width is a
	// bug upstream (the bit-size picker should have chosen a wider slot), so fail loud, never truncate.
	private ensureFits(value: number, max: number, width: string){
		if (value < 0 || value > max) {
			throw new Error(`value ${value} does not fit in ${width} (0..${max})`);
		}
	}

	write8Bits(value: number){
		this.ensureFits(value, Sizes.eightBitsMax, "8 bits");
		this.data.setUint8(this.offset, value);
		this.offset++;
	}

	write16Bits(value: number){
		this.ensureFits(value, Sizes.sixteenBitsMax, "16 bits");
		this.data.setUint16(this.offset, value, true);
		this.offset += 2;
	}

	write32Bits(value: number){
		this.ensureFits(value, Sizes.thirtyTwoBitsMax, "32 bits");
		this.data.setUint32(this.offset, value, true);
		this.offset += 4;
	}

	write64Bits(value: number){
		this.ensureFits(value, Sizes.maxInteger, "64 bits");
		const low = value >>> 0;
		const high = Math.floor(value / 2 ** 32) >>> 0;
		this.data.setUint32(this.offset, low, true);
		this.data.setUint32(this.offset + 4, high, true);
		this.offset += 8;
	}

	writeDimensions(item: Dimensions, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				this.write8Bits(item.length);
				this.write8Bits(item.width);
				this.write8Bits(item.height);
				break;
			case BitSize.Sixteen:
				this.write16Bits(item.length);
				this.write16Bits(item.width);
				this.write16Bits(item.height);
				break;
			case BitSize.ThirtyTwo:
				this.write32Bits(item.length);
				this.write32Bits(item.width);
				this.write32Bits(item.height);
				break;
			case BitSize.SixtyFour:
				this.write64Bits(item.length);
				this.write64Bits(item.width);
				this.write64Bits(item.height);
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}

	writeCoordinates(item: Coordinates, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				this.write8Bits(item.x);
				this.write8Bits(item.y);
				this.write8Bits(item.z);
				break;
			case BitSize.Sixteen:
				this.write16Bits(item.x);
				this.write16Bits(item.y);
				this.write16Bits(item.z);
				break;
			case BitSize.ThirtyTwo:
				this.write32Bits(item.x);
				this.write32Bits(item.y);
				this.write32Bits(item.z);
				break;
			case BitSize.SixtyFour:
				this.write64Bits(item.x);
				this.write64Bits(item.y);
				this.write64Bits(item.z);
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}


}
