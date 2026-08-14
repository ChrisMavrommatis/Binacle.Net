import {Coordinates, Dimensions, Header, Item, Layout} from "../models";
import {ProtocolReader} from "../ProtocolReader";
import {ProtocolWriter} from "../ProtocolWriter";
import {readRowMajor, writeRowMajor} from "./rowMajorCodec";
import {readColumnar, writeColumnar} from "./columnarCodec";

// Ports C#: LayoutCodecFactory. Picks the write/read pair for a Layout bit, and rejects an unknown code so a
// reserved layout is never silently ignored.
export type LayoutEncoder = (writer: ProtocolWriter, items: (Dimensions & Coordinates)[], header: Header) => void;
export type LayoutDecoder = (reader: ProtocolReader, items: Item[], header: Header) => void;

export function getLayoutEncoder(layout: Layout): LayoutEncoder {
	switch (layout) {
		case Layout.RowMajor:
			return writeRowMajor;
		case Layout.Columnar:
			return writeColumnar;
		default:
			throw new Error(`Layout ${layout} is not supported`);
	}
}

export function getLayoutDecoder(layout: Layout): LayoutDecoder {
	switch (layout) {
		case Layout.RowMajor:
			return readRowMajor;
		case Layout.Columnar:
			return readColumnar;
		default:
			throw new Error(`Layout ${layout} is not supported`);
	}
}
