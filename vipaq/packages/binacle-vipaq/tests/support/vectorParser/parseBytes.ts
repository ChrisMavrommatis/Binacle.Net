import {parseByte} from "./parseByte";

// Ports C#: VectorParser.ParseBytes. Many byte tokens -> a number array.
export function parseBytes(tokens: string[]): number[] {
	return tokens.map(parseByte);
}
