// Barrel for the vector parser. Compact-geometry grammar is the shared binacle-compact-notation package
// (one grammar for the whole repo; a bin is dimensions-only, so parseBin is parseDimensions). Encoding-info
// notation stays vipaq-local (it needs EncodingInfo/BitSize/Version). This barrel also adds the
// test-vector-only byte-token parsers. Consumers import from "../support/vectorParser".
export {parseByte} from "./parseByte";
export {parseBytes} from "./parseBytes";
export {parseDimensions as parseBin, parseDimensions, parseCoordinates, parseItems} from "binacle-compact-notation";
export {parseEncodingInfo} from "../../../src/encodingInfoNotation";
export {parseBitSize} from "./parseBitSize";
