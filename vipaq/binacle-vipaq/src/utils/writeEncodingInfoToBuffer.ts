import {encodingInfoToByte} from "./encodingInfoToByte";
import {EncodingInfo} from "../models";

export function writeEncodingInfoToBuffer(encodingInfo: EncodingInfo, buffer: Uint8Array<ArrayBuffer>): Uint8Array<ArrayBuffer> {
	const newBuffer = new Uint8Array(buffer.byteLength + 1);
	newBuffer[0] = encodingInfoToByte(encodingInfo);
	newBuffer.set(buffer, 1);
	return newBuffer;
}
