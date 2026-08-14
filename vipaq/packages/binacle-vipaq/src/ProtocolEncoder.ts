import {Bin, Coordinates, Dimensions, Header, Item, Width} from "./models";
import {getBodyLength, headerToBytes, Sizes, ViPaqFormatError} from "./utils";
import {getLayoutDecoder, getLayoutEncoder} from "./layouts";
import {CompressionCodec} from "./compression";
import {ProtocolReader} from "./ProtocolReader";
import {ProtocolWriter} from "./ProtocolWriter";

// Ports C#: ProtocolEncoder. The blind layer (PROTOCOL.md §1, §3, §6, §7). Handed a header, it obeys it and
// decides nothing; ViPaqSerializer chooses the header.
//
// The codec is a required constructor argument, no default, so a caller always says which one. noOpCodec runs
// the compressed path with the body left readable (§6.1 forbids comparing real compressed bytes). encode and
// decode are async because the browser compressor (CompressionStream) is async.
export class ProtocolEncoder {
	constructor(private readonly codec: CompressionCodec) {}

	// Produces a whole blob, obeying the header's compressed bit: the two header bytes, then the body (uint16
	// count, bin dimensions, then the items in the header's layout).
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

		// The whole body - count and contents (§3) - goes through the codec, which was resolved from the header.
		const body = await this.codec.compress(writer.buffer);

		const blob = new Uint8Array(Header.byteCount + body.length);
		blob.set(headerToBytes(header), 0);
		blob.set(body, Header.byteCount);
		return blob;
	}

	// Reads a body (everything after the two header bytes) back under the given header. One length check covers
	// both truncation and trailing bytes (PROTOCOL.md §7, steps 1 and 8), so every read below is in bounds.
	async decode(header: Header, rest: Uint8Array<ArrayBuffer>): Promise<{bin: Bin; items: Item[]}> {
		// The body has to be inflated first (§7, step 4): the item count lives inside it (§3).
		const body = await this.codec.decompress(rest);

		if (body.length < 2) {
			throw new ViPaqFormatError(`A body is at least 2 bytes (the item count), got ${body.length}`);
		}

		const reader = new ProtocolReader(new DataView(body.buffer, body.byteOffset, body.byteLength));
		const numberOfItems = reader.read16Bits();

		if (body.length !== getBodyLength(header, numberOfItems)) {
			throw new ViPaqFormatError(
				`Body length ${body.length} does not match the ${getBodyLength(header, numberOfItems)} bytes the header declares`,
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
