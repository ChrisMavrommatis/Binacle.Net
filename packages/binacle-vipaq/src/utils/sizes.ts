import {BitSize} from "../models/BitSize";

export default class Sizes {
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
}

