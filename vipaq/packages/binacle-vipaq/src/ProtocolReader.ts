import {Width} from "./models";

// Ports C#: ProtocolReader. Reads one value at a time, little-endian — the mirror of ProtocolWriter. Grouping is
// the caller's business (the layout codecs, ProtocolEncoder). Only 8- and 16-bit widths exist now, so the old
// 32/64-bit reads and their out-of-range guard are gone: a 16-bit value is always in [0, 65535].
export class ProtocolReader {
	private data: DataView<ArrayBuffer>;
	private offset: number;

	constructor(data: DataView<ArrayBuffer>) {
		this.data = data;
		this.offset = 0;
	}

	read8Bits(): number {
		const read = this.data.getUint8(this.offset);
		this.offset++;
		return read;
	}

	read16Bits(): number {
		const read = this.data.getUint16(this.offset, true);
		this.offset += 2;
		return read;
	}

	// Picks the read for a Width. The names match the Width enum.
	readValue(width: Width): number {
		switch (width) {
			case Width.Eight:
				return this.read8Bits();
			case Width.Sixteen:
				return this.read16Bits();
			default:
				throw new Error(`width ${width} is not supported`);
		}
	}
}
