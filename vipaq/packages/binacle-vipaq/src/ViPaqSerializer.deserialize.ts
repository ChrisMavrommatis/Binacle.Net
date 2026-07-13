import {DeserializedResult, Header} from "./models";
import {headerFromBytes, ViPaqFormatError} from "./utils";
import {noOpCodec} from "./compression";
import {ProtocolEncoder} from "./ProtocolEncoder";

// Ports C#: ViPaqSerializer.Deserialize. Splits off the two header bytes — which are never compressed — reads the
// header, then hands ProtocolEncoder the header plus everything after it. It refuses a compressed blob on
// purpose: the codec exists (deflate) and ProtocolEncoder could inflate it, but the serializer's compression
// policy is not baked in yet, so it stays symmetric with C# and reads raw blobs only.
export async function deserialize(data: Uint8Array<ArrayBuffer>): Promise<DeserializedResult> {
	if (!data || data.length < Header.byteCount) {
		throw new ViPaqFormatError("A blob is at least the two header bytes.");
	}

	const header = headerFromBytes(data[0], data[1]);

	if (header.compressed) {
		throw new Error(
			"This blob is compressed. The serializer does not read compressed blobs yet (PROTOCOL.md §6).",
		);
	}

	// Only ever reads raw blobs (compressed ones are refused above), so the codec is a NoOp — same as C#.
	const encoder = new ProtocolEncoder(noOpCodec);
	const {bin, items} = await encoder.decode(header, data.slice(Header.byteCount));

	return new DeserializedResult(bin, items);
}
