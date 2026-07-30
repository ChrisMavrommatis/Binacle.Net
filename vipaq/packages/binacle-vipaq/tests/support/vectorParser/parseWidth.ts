import {Width} from "../../../src/models";

const widthWords: Record<string, Width> = {
	Eight: Width.Eight,
	Sixteen: Width.Sixteen,
};

// "Eight" | "Sixteen" -> Width (the ExpectedWidth field). No direct C# counterpart — C# binds the enum name
// via JsonStringEnumConverter; TS maps it explicitly. The old ThirtyTwo/SixtyFour names are gone with the tiers.
export function parseWidth(name: string): Width {
	const width = widthWords[name];
	if (width === undefined) throw new Error(`Unknown width '${name}', expected 'Eight' or 'Sixteen'`);
	return width;
}
