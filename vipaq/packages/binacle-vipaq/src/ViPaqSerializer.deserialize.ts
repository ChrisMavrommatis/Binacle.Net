import {DeserializedResult, Header} from "./models";
import {headerFromBytes, ViPaqFormatError} from "./utils";
import {ProtocolEncoder} from "./ProtocolEncoder";

// Ports C#: ViPaqSerializer.Deserialize. Splits off the two header bytes — which are never compressed — reads the
// header, then hands ProtocolEncoder the header plus everything after it. A conformant blob whose header says
// compressed is still one this cannot read: the codec is deferred (PROTOCOL.md §6), so it says so plainly rather
// than guess.
export async function deserialize(data: Uint8Array<ArrayBuffer>): Promise<DeserializedResult> {
	if (!data || data.length < Header.byteCount) {
		throw new ViPaqFormatError("A blob is at least the two header bytes.");
	}

	const header = headerFromBytes(data[0], data[1]);

	if (header.compressed) {
		throw new Error(
			"This blob is compressed. The ViPaq compression codec is not chosen yet (PROTOCOL.md §6).",
		);
	}

	const encoder = new ProtocolEncoder();
	const {bin, items} = await encoder.decode(header, data.slice(Header.byteCount));

	return new DeserializedResult(bin, items);
}
