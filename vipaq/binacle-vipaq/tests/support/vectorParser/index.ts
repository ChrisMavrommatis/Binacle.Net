// Barrel for the vector parser. The compact-geometry and encoding-info grammar lives in the library
// (src/compactNotation, shared with the interop generator); this barrel re-exports it and adds the
// test-vector-only byte-token parsers. Consumers import from "../support/vectorParser".
export {parseByte} from "./parseByte";
export {parseBytes} from "./parseBytes";
export {parseBin, parseDimensions, parseCoordinates, parseItems, parseEncodingInfo} from "../../../src/compactNotation";
export {parseBitSize} from "./parseBitSize";
