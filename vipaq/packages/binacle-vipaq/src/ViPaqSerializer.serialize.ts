import {Coordinates, Dimensions} from "./models";
import {createHeader} from "./utils";
import {ProtocolEncoder} from "./ProtocolEncoder";

// Ports C#: ViPaqSerializer.Serialize. The choosing layer, and the only entry point a caller needs. It picks the
// header — the narrowest widths that hold each section, row-major, and (for now) always uncompressed — then
// hands ProtocolEncoder that header to obey. Compression is deferred (PROTOCOL.md §6), so nothing here ever sets
// the compressed bit; that is honest — the codec is not chosen at all yet.
export async function serialize(bin: Dimensions, items: (Dimensions & Coordinates)[]): Promise<Uint8Array<ArrayBuffer>> {
	if (!bin) {
		throw new Error("No Bin provided");
	}
	if (!items) {
		throw new Error("No items provided");
	}

	const header = createHeader(bin, items);
	const encoder = new ProtocolEncoder();
	return encoder.encode(header, bin, items);
}
