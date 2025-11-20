import ViPaqSerializer from "./vipaq-serializer";
import {EncodingInfoHelper} from "./helpers/EncodingInfoHelper";
import {EncodingInfo} from "./models/EncodingInfo";
import {Version} from "./models/Version";

ViPaqSerializer.prototype.Deserialize = function(data: Uint8Array) {
	if(!data || data.length < 1){
		throw new Error("Data is invalid or emtpy.");
	}

	let offset = 0;
	// Read the first byte (encoding info) before any decompression
	const firstByte = data[0];
	const restOfData = data.slice(1);

	const encodingInfo = EncodingInfoHelper.fromByte(firstByte);
	// EncodingInfoHelper.ThrowOnInvalidEncodingInfo<T>(encodingInfo);
	//
	// // Determine if the data is compressed
	const dataStream = getDecodingDataStream(restOfData, encodingInfo);
	//
	// using var protocolReader = new ProtocolReader<T>(dataStream);
	//
	// var numberOfItems = protocolReader.ReadUInt16();
	//
	// var bin = new TBin();
	// protocolReader.ReadDimensions<TBin, T>(ref bin, encodingInfo.BinDimensionsBitSize);
	//
	// var items = new List<TItem>();
	// for (int i = 0; i < numberOfItems; i++)
	// {
	// 	var item = new TItem();
	// 	protocolReader.ReadDimensions<TItem, T>(ref item, encodingInfo.ItemDimensionsBitSize);
	// 	protocolReader.ReadCoordinates<TItem, T>(ref item, encodingInfo.ItemCoordinatesBitSize);
	// 	items.Add(item);
	// }
	//
	// return (bin, items);

}

async function getDecodingDataStream(data: Uint8Array, encodingInfo: EncodingInfo){
	if(encodingInfo.version == Version.Uncompressed){
		return new DataView(data.buffer);
	}

	if(encodingInfo.version == Version.CompressedGzip){
		const blob = new Blob([data.buffer]);
		const ds = new DecompressionStream('gzip');
		const decompressedStream = blob.stream().pipeThrough(ds);
		const reader = decompressedStream.getReader();
		const chunks: Uint8Array[] = [];
		let done = false;
		while (!done) {
			const { value, done: readerDone } = await reader.read();
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
	throw new Error(`version ${encodingInfo.version} is not supported`);

}
