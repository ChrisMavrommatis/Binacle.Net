import {encodingInfoFromByte, getDecodingDataStream} from "./utils";
import {Bin, DeserializedResult, Item} from "./models";
import {ProtocolReader} from "./ProtocolReader";


export async function deserialize(data: Uint8Array<ArrayBuffer>): Promise<DeserializedResult> {
	if (!data || data.length < 1) {
		throw new Error("Data is invalid or empty.");
	}

	// Read the first byte (encoding info) before any decompression
	const firstByte = data[0];
	const restOfData = data.slice(1);

	const encodingInfo = encodingInfoFromByte(firstByte);

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


