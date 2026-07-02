// Ports C#: VectorParser.ParseThree (private). Internal helper — NOT re-exported from index.ts.
// "A{separator}B{separator}C" -> three numbers. Dimensions/bin split on 'x'; coordinates split on ','.
// A leading '-' is allowed so invalid-input vectors can carry negatives.
export function parseThree(compact: string, separator: string): [number, number, number] {
	const parts = compact.split(separator);
	if (parts.length !== 3) throw new Error(`'${compact}' must be three values separated by '${separator}'.`);
	return [Number(parts[0]), Number(parts[1]), Number(parts[2])];
}
