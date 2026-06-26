import {BitSize} from "../models";

export class Sizes {
	public static bitSizes: Record<string, BitSize> = {
		// For byte, sbyte, short, ushort, int, uint, long, ulong, use 'number'
		'byte': BitSize.Eight,
		'sbyte': BitSize.Eight,
		'short': BitSize.Sixteen,
		'ushort': BitSize.Sixteen,
		'int': BitSize.ThirtyTwo,
		'uint': BitSize.ThirtyTwo,
		'long': BitSize.SixtyFour,
		'ulong': BitSize.SixtyFour,
	};

	public static byteMaxSize = 255;
	public static uShortMaxValue = 65_535;
	public static uIntMaxValue = 4_294_967_295;
	public static uLongMaxValue = 9_223_372_036_854_775_807;

	// ViPaq's interoperable integer ceiling. The largest integer every target runtime holds exactly is
	// 2^53 - 1 (Number.MAX_SAFE_INTEGER). C# ulong can hold more, but values above this are outside the
	// protocol — see vipaq/PROTOCOL.md. All dimensions and coordinates must be in [0, maxInteger].
	public static maxInteger = 9_007_199_254_740_991;
}

