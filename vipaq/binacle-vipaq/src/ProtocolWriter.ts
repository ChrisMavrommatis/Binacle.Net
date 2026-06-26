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

	writeByte(value: number){
		this.ensureFits(value, Sizes.byteMaxSize, "a byte");
		this.data.setUint8(this.offset, value);
		this.offset++;
	}

	writeUInt16(value: number){
		this.ensureFits(value, Sizes.uShortMaxValue, "a uint16");
		this.data.setUint16(this.offset, value, true);
		this.offset += 2;
	}

	writeUInt32(value: number){
		this.ensureFits(value, Sizes.uIntMaxValue, "a uint32");
		this.data.setUint32(this.offset, value, true);
		this.offset += 4;
	}

	writeUInt64(value: number){
		this.ensureFits(value, Sizes.maxInteger, "a uint64");
		const low = value >>> 0;
		const high = Math.floor(value / 2 ** 32) >>> 0;
		this.data.setUint32(this.offset, low, true);
		this.data.setUint32(this.offset + 4, high, true);
		this.offset += 8;
	}

	writeDimensions(item: Dimensions, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				this.writeByte(item.length);
				this.writeByte(item.width);
				this.writeByte(item.height);
				break;
			case BitSize.Sixteen:
				this.writeUInt16(item.length);
				this.writeUInt16(item.width);
				this.writeUInt16(item.height);
				break;
			case BitSize.ThirtyTwo:
				this.writeUInt32(item.length);
				this.writeUInt32(item.width);
				this.writeUInt32(item.height);
				break;
			case BitSize.SixtyFour:
				this.writeUInt64(item.length);
				this.writeUInt64(item.width);
				this.writeUInt64(item.height);
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}

	writeCoordinates(item: Coordinates, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				this.writeByte(item.x);
				this.writeByte(item.y);
				this.writeByte(item.z);
				break;
			case BitSize.Sixteen:
				this.writeUInt16(item.x);
				this.writeUInt16(item.y);
				this.writeUInt16(item.z);
				break;
			case BitSize.ThirtyTwo:
				this.writeUInt32(item.x);
				this.writeUInt32(item.y);
				this.writeUInt32(item.z);
				break;
			case BitSize.SixtyFour:
				this.writeUInt64(item.x);
				this.writeUInt64(item.y);
				this.writeUInt64(item.z);
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}


}
