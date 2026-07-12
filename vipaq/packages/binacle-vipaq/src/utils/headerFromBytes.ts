import {Header, Layout, Version, Width} from "../models";
import {ViPaqFormatError} from "./viPaqFormatError";

// Ports C#: Header.FromBytes. Reads the two header bytes back (PROTOCOL.md §7, steps 2 and 3). Every rejection
// here means a malformed blob, not a caller bug, so it throws ViPaqFormatError.
export function headerFromBytes(formByte: number, widthsByte: number): Header {
	const version = (formByte & 0b1100_0000) >> 6;
	if (version !== Version.Version1) {
		throw new ViPaqFormatError(
			`Unsupported version ${version}, this implementation reads ${Version.Version1}`,
		);
	}

	if ((formByte & 0b0000_1111) !== 0) {
		throw new ViPaqFormatError("Reserved bits 3-0 of header byte 0 must be zero");
	}

	if ((widthsByte & 0b0000_0011) !== 0) {
		throw new ViPaqFormatError("Reserved bits 1-0 of header byte 1 must be zero");
	}

	return new Header(
		version,
		(formByte & 0b0010_0000) !== 0,
		(formByte & 0b0001_0000) >> 4 as Layout,
		toWidth((widthsByte & 0b1100_0000) >> 6, "bin dimensions"),
		toWidth((widthsByte & 0b0011_0000) >> 4, "item dimensions"),
		toWidth((widthsByte & 0b0000_1100) >> 2, "item coordinates"),
	);
}

// Width codes 2 and 3 are reserved. A decoder must reject them (PROTOCOL.md §4).
function toWidth(code: number, section: string): Width {
	if (code === Width.Eight || code === Width.Sixteen) {
		return code;
	}
	throw new ViPaqFormatError(`Reserved width code ${code} for ${section}`);
}
