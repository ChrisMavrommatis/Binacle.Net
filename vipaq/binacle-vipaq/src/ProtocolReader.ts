import {Bin, BitSize, Coordinates, Dimensions} from "./models";

export class ProtocolReader {
	private data: DataView<ArrayBuffer>;
	private offset: number;
	constructor(data: DataView<ArrayBuffer>) {
		this.data = data;
		this.offset = 0;
	}

	readByte(){
		const read = this.data.getUint8(this.offset);
		this.offset++;
		return read;
	}

	readUint16() {
		const read = this.data.getUint16(this.offset, true);
		this.offset += 2;
		return read;
	}

	readUint32(){
		const read = this.data.getUint32(this.offset, true);
		this.offset += 4;
		return read;
	}

	readUint64(){
		const left = this.data.getUint32(this.offset, true);
		const right = this.data.getUint32(this.offset + 4, true);
		this.offset += 8;

		const result = left + 2**32*right;
		return result;
	}

	readDimensions(item: Dimensions, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				item.length = this.readByte();
				item.width = this.readByte();
				item.height = this.readByte();
				break;
			case BitSize.Sixteen:
				item.length = this.readUint16();
				item.width = this.readUint16();
				item.height = this.readUint16();
				break;
			case BitSize.ThirtyTwo:
				item.length = this.readUint32();
				item.width = this.readUint32();
				item.height = this.readUint32();
				break;
			case BitSize.SixtyFour:
				item.length = this.readUint64();
				item.width = this.readUint64();
				item.height = this.readUint64();
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}

	readCoordinates(item: Coordinates, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				item.x = this.readByte();
				item.y = this.readByte();
				item.z = this.readByte();
				break;
			case BitSize.Sixteen:
				item.x = this.readUint16();
				item.y = this.readUint16();
				item.z = this.readUint16();
				break;
			case BitSize.ThirtyTwo:
				item.x = this.readUint32();
				item.y = this.readUint32();
				item.z = this.readUint32();
				break;
			case BitSize.SixtyFour:
				item.x = this.readUint64();
				item.y = this.readUint64();
				item.z = this.readUint64();
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}


}
