import {BitSize, Coordinates, Dimensions} from "./models";
import {Sizes} from "./utils";

export class ProtocolReader {
	private data: DataView<ArrayBuffer>;
	private offset: number;
	constructor(data: DataView<ArrayBuffer>) {
		this.data = data;
		this.offset = 0;
	}

	read8Bits(){
		const read = this.data.getUint8(this.offset);
		this.offset++;
		return read;
	}

	read16Bits() {
		const read = this.data.getUint16(this.offset, true);
		this.offset += 2;
		return read;
	}

	read32Bits(){
		const read = this.data.getUint32(this.offset, true);
		this.offset += 4;
		return read;
	}

	read64Bits(){
		const left = this.data.getUint32(this.offset, true);
		const right = this.data.getUint32(this.offset + 4, true);
		this.offset += 8;

		const result = left + 2**32*right;
		// A C#-made buffer can hold 64-bit values above what JS represents exactly. Refuse them instead
		// of returning a silently-rounded number — see vipaq/PROTOCOL.md.
		if (result > Sizes.maxInteger) {
			throw new Error(`decoded value exceeds the max supported value (${Sizes.maxInteger})`);
		}
		return result;
	}

	readDimensions(item: Dimensions, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				item.length = this.read8Bits();
				item.width = this.read8Bits();
				item.height = this.read8Bits();
				break;
			case BitSize.Sixteen:
				item.length = this.read16Bits();
				item.width = this.read16Bits();
				item.height = this.read16Bits();
				break;
			case BitSize.ThirtyTwo:
				item.length = this.read32Bits();
				item.width = this.read32Bits();
				item.height = this.read32Bits();
				break;
			case BitSize.SixtyFour:
				item.length = this.read64Bits();
				item.width = this.read64Bits();
				item.height = this.read64Bits();
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}

	readCoordinates(item: Coordinates, bitSize: BitSize){
		switch (bitSize) {
			case BitSize.Eight:
				item.x = this.read8Bits();
				item.y = this.read8Bits();
				item.z = this.read8Bits();
				break;
			case BitSize.Sixteen:
				item.x = this.read16Bits();
				item.y = this.read16Bits();
				item.z = this.read16Bits();
				break;
			case BitSize.ThirtyTwo:
				item.x = this.read32Bits();
				item.y = this.read32Bits();
				item.z = this.read32Bits();
				break;
			case BitSize.SixtyFour:
				item.x = this.read64Bits();
				item.y = this.read64Bits();
				item.z = this.read64Bits();
				break;
			default:
				throw new Error(`BitSize ${bitSize} is not supported`);
		}
	}


}
