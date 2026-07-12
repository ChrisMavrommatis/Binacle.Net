import {Coordinates, Dimensions, Header, Layout, Version, Width} from "../models";
import {Sizes} from "./sizes";
import {getDimensionsWidth} from "./getDimensionsWidth";
import {getCoordinatesWidth} from "./getCoordinatesWidth";

// Ports C#: Header.Create. The header for this data at the library's default form: the narrowest widths that
// hold each section (PROTOCOL.md §4), uncompressed and row-major. The three sections are sized independently — a
// big bin can hold small items at large coordinates — so they genuinely disagree. With no items, both item
// widths stay Eight, which is what §4 requires of an empty blob.
//
// Compression and layout are left at their defaults (raw, row-major): whether compressing pays can only be known
// after trying it (deferred, PROTOCOL.md §6), and layout is unmeasured. A caller that wants a different form
// takes this header and changes those fields — the interop generator does exactly that for the columnar cases.
export function createHeader(bin: Dimensions, items: (Dimensions & Coordinates)[]): Header {
	if (items.length > Sizes.maxItemCount) {
		throw new Error(`Items cannot be more than ${Sizes.maxItemCount}`);
	}

	let itemDimensionsWidth = Width.Eight;
	let itemCoordinatesWidth = Width.Eight;
	for (const item of items) {
		const localItemDimensionsWidth = getDimensionsWidth(item);
		if (localItemDimensionsWidth > itemDimensionsWidth) {
			itemDimensionsWidth = localItemDimensionsWidth;
		}
		const localItemCoordinatesWidth = getCoordinatesWidth(item);
		if (localItemCoordinatesWidth > itemCoordinatesWidth) {
			itemCoordinatesWidth = localItemCoordinatesWidth;
		}
	}

	return new Header(
		Version.Version1,
		false,
		Layout.RowMajor,
		getDimensionsWidth(bin),
		itemDimensionsWidth,
		itemCoordinatesWidth,
	);
}
