import {Coordinates, Dimensions, Header, Layout, Version, Width} from "../models";
import {Sizes} from "./sizes";
import {getDimensionsWidth} from "./getDimensionsWidth";
import {getCoordinatesWidth} from "./getCoordinatesWidth";

// Ports C#: Header.Create. Narrowest widths that hold each section (PROTOCOL.md §4), uncompressed and
// row-major. The three sections are sized independently because they disagree: a big bin can hold small items
// at large coordinates. With no items both item widths stay Eight, as §4 requires.
//
// A caller wanting a different form takes this header and changes `compressed` / `layout`.
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
