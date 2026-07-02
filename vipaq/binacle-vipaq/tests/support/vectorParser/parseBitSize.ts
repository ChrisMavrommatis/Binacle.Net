import {BitSize} from "../../../src/models";

const bitSizeWords: Record<string, BitSize> = {
	Eight: BitSize.Eight,
	Sixteen: BitSize.Sixteen,
	ThirtyTwo: BitSize.ThirtyTwo,
	SixtyFour: BitSize.SixtyFour,
};

// "Eight" | "Sixteen" | "ThirtyTwo" | "SixtyFour" -> BitSize (the ExpectedBitSize field). No direct C#
// counterpart — C# binds the enum name via JsonStringEnumConverter; TS maps it explicitly.
export function parseBitSize(name: string): BitSize {
	return bitSizeWords[name];
}
