import {DeserializedResult, Header} from "./models";
import {headerFromBytes, ViPaqFormatError} from "./utils";
import {ProtocolEncoder} from "./ProtocolEncoder";
import {resolveCodec} from "./resolveCodec";

// Ports C#: ViPaqSerializer.Deserialize. Splits off the two header bytes — which are never compressed — reads the
// header, resolves the codec from it (resolveCodec), and hands both to the encoder. Raw DEFLATE when the
// compressed bit is set, otherwise a NoOp that passes the raw body through (§6).
export async function deserialize(data: Uint8Array<ArrayBuffer>): Promise<DeserializedResult> {
	if (!data || data.length < Header.byteCount) {
		throw new ViPaqFormatError("A blob is at least the two header bytes.");
	}

	const header = headerFromBytes(data[0], data[1]);
	const codec = resolveCodec(header);
	const {bin, items} = await new ProtocolEncoder(codec).decode(header, data.slice(Header.byteCount));

	return new DeserializedResult(bin, items);
}
