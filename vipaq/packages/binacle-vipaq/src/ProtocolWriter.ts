import {Width} from "./models";
import {Sizes} from "./utils";

// Ports C#: ProtocolWriter. Writes one value at a time, little-endian. It does not know what a dimension or a
// coordinate is: grouping values into triples, and the order they go in, is the caller's business — the layout
// codecs for the items, ProtocolEncoder for the bin. Only 8- and 16-bit widths exist now; the old 32/64-bit
// writes are gone.
export class ProtocolWriter {
	private offset: number;
	public buffer: Uint8Array<ArrayBuffer>;
	private data: DataView<ArrayBuffer>;

	constructor(bufferSize: number) {
		this.buffer = new Uint8Array(bufferSize);
		this.data = new DataView(this.buffer.buffer);
		this.offset = 0;
	}

	// Range-check before every write, like C#'s CreateChecked. A value that does not fit the width is a bug
	// upstream (the width picker should have chosen a wider slot), so fail loud, never truncate.
	private ensureFits(value: number, max: number, width: string) {
		if (value < 0 || value > max) {
			throw new Error(`value ${value} does not fit in ${width} (0..${max})`);
		}
	}

	write8Bits(value: number) {
		this.ensureFits(value, Sizes.eightBitsMax, "8 bits");
		this.data.setUint8(this.offset, value);
		this.offset++;
	}

	write16Bits(value: number) {
		this.ensureFits(value, Sizes.sixteenBitsMax, "16 bits");
		this.data.setUint16(this.offset, value, true);
		this.offset += 2;
	}

	// Picks the write for a Width. The names match the Width enum.
	writeValue(value: number, width: Width) {
		switch (width) {
			case Width.Eight:
				this.write8Bits(value);
				break;
			case Width.Sixteen:
				this.write16Bits(value);
				break;
			default:
				throw new Error(`width ${width} is not supported`);
		}
	}
}
