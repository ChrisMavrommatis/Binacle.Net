import Bin from "./models/Bin";
import Item from "./models/Item";
import createEncodingInfo from "./utils/createEncodingInfo";
import {ProtocolWriter} from "./ProtocolWriter";
import getBufferSize from "./utils/getBufferSize";
import Sizes from "./utils/sizes";
import {Version} from "./models/Version";
import {compressBuffer, writeEncodingInfoToBuffer} from "./utils/encodingUtils";


export async function serialize(bin: Bin, items: Item[]): Promise<Uint8Array<ArrayBuffer>> {
	if (!bin) {
		throw new Error("No Bin provided");
	}
	if(!items  || items.length < 1)	{
		throw new Error("No items provided");
	}
	const encodingInfo = createEncodingInfo(bin, items);
	const numberOfItems = items.length;

	const bufferSize = getBufferSize(encodingInfo, numberOfItems);

	// encoding info to be written at the end
	const protocolWriter = new ProtocolWriter(bufferSize - 1);
	protocolWriter.writeUInt16(numberOfItems);
	protocolWriter.writeDimensions(bin, encodingInfo.binDimensionsBitSize);

	for(let item of items)
	{
		protocolWriter.writeDimensions(item, encodingInfo.itemDimensionsBitSize);
		protocolWriter.writeCoordinates(item, encodingInfo.itemCoordinatesBitSize);
	}

	const shouldCompress = bufferSize > Sizes.byteMaxSize;
	if (!shouldCompress)
	{
		return writeEncodingInfoToBuffer(encodingInfo, protocolWriter.buffer);
	}
	encodingInfo.version = Version.CompressedGzip;
	const compressedBuffer = await compressBuffer(protocolWriter.buffer);
	return writeEncodingInfoToBuffer(encodingInfo, compressedBuffer);
}

