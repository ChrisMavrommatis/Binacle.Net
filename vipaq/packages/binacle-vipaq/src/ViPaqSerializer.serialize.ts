import {Coordinates, Dimensions, Layout} from "./models";
import {createHeader} from "./utils";
import {ProtocolEncoder} from "./ProtocolEncoder";
import {resolveCodec} from "./resolveCodec";
import {ViPaqSerializationOptions} from "./ViPaqSerializationOptions";

// Ports C#: ViPaqSerializer.Serialize. The choosing layer, and the only entry point a caller needs. It picks
// the widths; the caller's options set layout and compression. The codec follows from the header.
export async function serialize(
	bin: Dimensions,
	items: (Dimensions & Coordinates)[],
	options?: ViPaqSerializationOptions,
): Promise<Uint8Array<ArrayBuffer>> {
	if (!bin) {
		throw new Error("No Bin provided");
	}
	if (!items) {
		throw new Error("No items provided");
	}

	// createHeader picks the widths; the caller's options set the rest.
	const header = createHeader(bin, items);
	header.layout = options?.layout ?? Layout.RowMajor;
	header.compressed = options?.compress ?? false;

	const codec = resolveCodec(header);
	return new ProtocolEncoder(codec).encode(header, bin, items);
}
