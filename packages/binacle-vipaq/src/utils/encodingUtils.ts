import EncodingInfo from "../models/EncodingInfo";
import {Version} from "../models/Version";
import Sizes from "./sizes";

export function encodingInfoFromByte(firstByte: number): EncodingInfo {
	const version = (firstByte & 0b11000000) >> 6;
	const binDimensionsBitSize = (firstByte & 0b00110000) >> 4;
	const itemDimensionsBitSize = (firstByte & 0b00001100) >> 2;
	const itemCoordinatesBitSize = firstByte & 0b00000011;

	return new EncodingInfo(
		version,
		binDimensionsBitSize,
		itemDimensionsBitSize,
		itemCoordinatesBitSize
	);
}

export function  encodingInfoToByte(encodingInfo: EncodingInfo): number {
	let encodingInfoByte = 0;
	encodingInfoByte |= (encodingInfo.version << 6);
	encodingInfoByte |= (encodingInfo.binDimensionsBitSize << 4);
	encodingInfoByte |= (encodingInfo.itemDimensionsBitSize << 2);
	encodingInfoByte |= encodingInfo.itemCoordinatesBitSize;
	return encodingInfoByte;
}


export async function getDecodingDataStream(data: Uint8Array<ArrayBuffer>, encodingInfo: EncodingInfo): Promise<DataView<ArrayBuffer>> {

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

export async function compressBuffer(data: Uint8Array<ArrayBuffer>) : Promise<Uint8Array<ArrayBuffer>> {

	const compressionStream = new CompressionStream('gzip');
	const blob = new Blob([data.buffer]);
	const compressedStream = blob.stream().pipeThrough(compressionStream);
	const reader = compressedStream.getReader();
	const chunks: Uint8Array[] = [];
	let done = false;
	while (!done) {
		const {value, done: readerDone} = await reader.read();
		if (value) chunks.push(value);
		done = readerDone;
	}
	const compressed = new Uint8Array(chunks.reduce((acc, chunk) => acc + chunk.length, 0));
	let offset = 0;
	for (const chunk of chunks) {
		compressed.set(chunk, offset);
		offset += chunk.length;
	}
	return compressed;
}


export function writeEncodingInfoToBuffer(encodingInfo: EncodingInfo, buffer: Uint8Array<ArrayBuffer>): Uint8Array<ArrayBuffer>{
	const newBuffer = new Uint8Array(buffer.byteLength + 1);
	newBuffer[0] = encodingInfoToByte(encodingInfo);
	newBuffer.set(buffer, 1);
	return newBuffer;
}
