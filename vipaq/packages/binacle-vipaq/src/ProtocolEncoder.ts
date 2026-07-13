import {Bin, Coordinates, Dimensions, Header, Item, Width} from "./models";
import {getBodyLength, headerToBytes, Sizes, ViPaqFormatError} from "./utils";
import {getLayoutDecoder, getLayoutEncoder} from "./layouts";
import {CompressionCodec} from "./compression";
import {ProtocolReader} from "./ProtocolReader";
import {ProtocolWriter} from "./ProtocolWriter";

// Ports C#: ProtocolEncoder. The blind layer (PROTOCOL.md §1, §3, §6, §7). It is handed a header and it obeys it
// — the widths to work at, the layout to work in, and whether to compress. It decides nothing; choosing the
// header is ViPaqSerializer's job. Encode and decode live together because they are one agreement read in two
// directions.
//
// The codec is a required constructor argument, exactly like C#: no default, so a caller always says which one.
// Hand it noOpCodec and the compressed path runs with the body left readable (§6.1 forbids comparing real
// compressed bytes); hand it deflateCodec/gzipCodec for a real stream. encode/decode are async because the
// browser compressor (CompressionStream) is async.
export class ProtocolEncoder {
	constructor(private readonly codec: CompressionCodec) {}

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

		// The item count is inside the body (§3), so the whole body — count and contents — is compressed as one.
		const body = header.compressed ? await this.codec.compress(writer.buffer) : writer.buffer;

		const blob = new Uint8Array(Header.byteCount + body.length);
		blob.set(headerToBytes(header), 0);
		blob.set(body, Header.byteCount);
		return blob;
	}

	// Reads a body (everything after the two header bytes) back under the given header. One length check covers
	// both truncation and trailing bytes (PROTOCOL.md §7, steps 1 and 8), so every read below is in bounds.
	async decode(header: Header, rest: Uint8Array<ArrayBuffer>): Promise<{bin: Bin; items: Item[]}> {
		// The item count lives inside the compressed body (§3), so it cannot be read until the body is inflated
		// (§7, step 4). When not compressed the body is `rest` unchanged.
		const body = header.compressed ? await this.codec.decompress(rest) : rest;

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
