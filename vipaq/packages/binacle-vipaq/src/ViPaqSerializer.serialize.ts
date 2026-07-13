import {Coordinates, Dimensions} from "./models";
import {createHeader} from "./utils";
import {noOpCodec} from "./compression";
import {ProtocolEncoder} from "./ProtocolEncoder";

// Ports C#: ViPaqSerializer.Serialize. The choosing layer, and the only entry point a caller needs. It picks the
// header — the narrowest widths that hold each section, row-major, and (for now) always uncompressed — then
// hands ProtocolEncoder that header to obey. The codec is chosen (deflate), but turning compression *on* in the
// serializer is not baked in yet, so createHeader never sets the compressed bit and every blob is raw.
export async function serialize(bin: Dimensions, items: (Dimensions & Coordinates)[]): Promise<Uint8Array<ArrayBuffer>> {
	if (!bin) {
		throw new Error("No Bin provided");
	}
	if (!items) {
		throw new Error("No items provided");
	}

	const header = createHeader(bin, items);
	// Never compresses (the header's compressed bit stays off), so the codec is a NoOp — same as C#.
	const encoder = new ProtocolEncoder(noOpCodec);
	return encoder.encode(header, bin, items);
}
