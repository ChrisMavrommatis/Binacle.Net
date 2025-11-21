import {EncodingInfoHelper} from "./helpers/EncodingInfoHelper";
import {ProtocolReader} from "./ProtocolReader";
import {EncodingInfo} from "./models/EncodingInfo";
import {Version} from "./models/Version";
import Bin from "./models/Bin";
import Item from "./models/Item";
import DeserializedResult from "./models/DeserializedResult";


export async function deserialize(data: Uint8Array<ArrayBuffer>): Promise<DeserializedResult> {
	if (!data || data.length < 1) {
		throw new Error("Data is invalid or empty.");
	}

	// Read the first byte (encoding info) before any decompression
	const firstByte = data[0];
	const restOfData = data.slice(1);

	const encodingInfo = EncodingInfoHelper.fromByte(firstByte);
	// EncodingInfoHelper.ThrowOnInvalidEncodingInfo<T>(encodingInfo);

	// Determine if the data is compressed
	const dataStream = await getDecodingDataStream(restOfData, encodingInfo);
	const protocolReader = new ProtocolReader(dataStream);
	const numberOfItems = protocolReader.readUint16();

	const bin = new Bin();
	protocolReader.readDimensions(bin, encodingInfo.binDimensionsBitSize);

	const items: Item[] = [];
	for (let i = 0; i < numberOfItems; i++) {
		const item = new Item();
		protocolReader.readDimensions(item, encodingInfo.itemDimensionsBitSize);
		protocolReader.readCoordinates(item, encodingInfo.itemCoordinatesBitSize);
		items.push(item);
	}

	return new DeserializedResult(bin, items);
}

async function getDecodingDataStream(data: Uint8Array<ArrayBuffer>, encodingInfo: EncodingInfo): Promise<DataView<ArrayBuffer>> {

	if (encodingInfo.version == Version.Uncompressed) {
		return new DataView(data.buffer);
	}

	if (encodingInfo.version == Version.CompressedGzip) {
		const blob = new Blob([data.buffer]);
		const ds = new DecompressionStream('gzip');
		const decompressedStream = blob.stream().pipeThrough(ds);
		const reader = decompressedStream.getReader();
		const chunks: Uint8Array[] = [];
		let done = false;
		while (!done) {
			const {value, done: readerDone} = await reader.read();
			if (value) chunks.push(value);
			done = readerDone;
		}
		const decompressed = new Uint8Array(chunks.reduce((acc, chunk) => acc + chunk.length, 0));
		let offset = 0;
		for (const chunk of chunks) {
			decompressed.set(chunk, offset);
			offset += chunk.length;
		}
		return new DataView(decompressed.buffer);
	}

	throw new Error(`Version ${encodingInfo.version} is not supported`);

}
