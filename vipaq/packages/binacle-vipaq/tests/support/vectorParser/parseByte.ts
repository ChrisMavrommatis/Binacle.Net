// Ports C#: VectorParser.ParseByte. "0x0A" (hex) or "0b00_01_00_00" (grouped binary) -> one byte.
// Underscores are separators.
export function parseByte(token: string): number {
	const normalized = token.replace(/_/g, "");
	if (normalized.startsWith("0x")) return parseInt(normalized.slice(2), 16);
	if (normalized.startsWith("0b")) return parseInt(normalized.slice(2), 2);
	throw new Error(`Byte token '${token}' must start with 0x or 0b.`);
}
