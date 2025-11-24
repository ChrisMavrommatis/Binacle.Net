import Bin from "./models/Bin";
import {BitSize} from "./models/BitSize";
import Item from "./models/Item";

export class ProtocolWriter {
	private offset: number;
	public buffer: Uint8Array<ArrayBuffer>;
	private data: DataView<ArrayBuffer>;

	constructor(bufferSize: number) {
		this.buffer = new Uint8Array(bufferSize);
		this.data = new DataView(this.buffer.buffer);
		this.offset = 0;
	}

	writeByte(value: number){
		this.data.setUint8(this.offset, value);
		this.offset++;
	}

	writeUInt16(value: number){
		this.data.setUint16(this.offset, value, true);
		this.offset += 2;
	}

	writeUInt32(value: number){
		this.data.setUint32(this.offset, value, true);
		this.offset += 4;
	}

	writeUInt64(value: number){
		const low = value >>> 0;
		const high = Math.floor(value / 2 ** 32) >>> 0;
		this.data.setUint32(this.offset, low, true);
		this.data.setUint32(this.offset + 4, high, true);
		this.offset += 8;
	}

	writeDimensions(item: Bin, bitSize: BitSize){
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

	writeCoordinates(item: Item, bitSize: BitSize){
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
