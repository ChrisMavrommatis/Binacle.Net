import {Bin, Coordinates, Dimensions, Header, Item, Width} from "./models";
import {getBodyLength, headerToBytes, Sizes, ViPaqFormatError} from "./utils";
import {getLayoutDecoder, getLayoutEncoder} from "./layouts";
import {ProtocolReader} from "./ProtocolReader";
import {ProtocolWriter} from "./ProtocolWriter";

// Ports C#: ProtocolEncoder. The blind layer (PROTOCOL.md §1, §3, §7). It is handed a header and it obeys it —
// the widths to work at and the layout to work in. It decides nothing; choosing the header is ViPaqSerializer's
// job. Encode and decode live together because they are one agreement read in two directions.
//
// Compression is deferred (PROTOCOL.md §6), so there is no codec here yet: this reads and writes raw bodies
// only, and a compressed header never reaches it (ViPaqSerializer rejects one first). encode/decode are async so
// the signatures do not have to change when a gzip codec is added.
export class ProtocolEncoder {
	// Produces a whole blob: the two header bytes, then the body (uint16 count, bin dimensions, then the items
	// in the header's layout).
	async encode(header: Header, bin: Dimensions, items: (Dimensions & Coordinates)[]): Promise<Uint8Array<ArrayBuffer>> {
		if (items.length > Sizes.maxItemCount) {
			throw new Error(`Items cannot be more than ${Sizes.maxItemCount}`);
		}
		if (items.length === 0 &&
			(header.itemDimensionsWidth !== Width.Eight || header.itemCoordinatesWidth !== Width.Eight)) {
			throw new Error("With no items, the item widths must be Eight");
		}

		const bodyLength = getBodyLength(header, items.length);
		const writer = new ProtocolWriter(bodyLength);

		writer.write16Bits(items.length);
		writer.writeValue(bin.length, header.binDimensionsWidth);
		writer.writeValue(bin.width, header.binDimensionsWidth);
		writer.writeValue(bin.height, header.binDimensionsWidth);

		getLayoutEncoder(header.layout)(writer, items, header);

		const blob = new Uint8Array(Header.byteCount + bodyLength);
		blob.set(headerToBytes(header), 0);
		blob.set(writer.buffer, Header.byteCount);
		return blob;
	}

	// Reads a body (everything after the two header bytes) back under the given header. One length check covers
	// both truncation and trailing bytes (PROTOCOL.md §7, steps 1 and 8), so every read below is in bounds.
	async decode(header: Header, rest: Uint8Array<ArrayBuffer>): Promise<{bin: Bin; items: Item[]}> {
		if (rest.length < 2) {
			throw new ViPaqFormatError(`A body is at least 2 bytes (the item count), got ${rest.length}`);
		}

		const reader = new ProtocolReader(new DataView(rest.buffer, rest.byteOffset, rest.byteLength));
		const numberOfItems = reader.read16Bits();

		if (rest.length !== getBodyLength(header, numberOfItems)) {
			throw new ViPaqFormatError(
				`Body length ${rest.length} does not match the ${getBodyLength(header, numberOfItems)} bytes the header declares`,
			);
		}

		const bin = new Bin();
		bin.length = reader.readValue(header.binDimensionsWidth);
		bin.width = reader.readValue(header.binDimensionsWidth);
		bin.height = reader.readValue(header.binDimensionsWidth);

		const items: Item[] = [];
		for (let index = 0; index < numberOfItems; index++) {
			items.push(new Item());
		}
		getLayoutDecoder(header.layout)(reader, items, header);

		return {bin, items};
	}
}
