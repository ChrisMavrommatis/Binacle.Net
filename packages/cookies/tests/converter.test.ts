import converter from "../src/converter";

// The two regexes are upstream js-cookie's. They decide which characters survive a round trip literally
// and which come back percent-encoded, so the character sets are pinned here rather than described.

describe("write", () => {
	test("percent-encodes a space", () => {
		const value = "a b";

		const written = converter.write(value);

		expect(written).toBe("a%20b");
	});

	test("leaves the characters js-cookie deliberately keeps", () => {
		const value = "#$&+/:<=>?@[]^`{|}";

		const written = converter.write(value);

		expect(written).toBe("#$&+/:<=>?@[]^`{|}");
	});

	test("encodes the characters that would break the cookie string", () => {
		const value = "\";,\\%";

		const written = converter.write(value);

		expect(written).toBe("%22%3B%2C%5C%25");
	});
});

describe("read", () => {
	test("decodes a percent-encoded space", () => {
		const value = "a%20b";

		const read = converter.read(value);

		expect(read).toBe("a b");
	});

	// \u00e9 is two UTF-8 bytes, so it arrives as two percent groups. The + in the regex is what
	// makes them decode as one run instead of byte by byte.
	test("decodes a multi-byte sequence as one run", () => {
		const value = "%C3%A9";

		const read = converter.read(value);

		expect(read).toBe("\u00e9");
	});

	test("strips the quotes around a quoted value", () => {
		const value = "\"quoted\"";

		const read = converter.read(value);

		expect(read).toBe("quoted");
	});
});

describe("round trip", () => {
	test.each([
		"plain",
		"a b",
		"#$&+/:<=>?@[]^`{|}",
		"\";,\\%",
		"caf\u00e9",
	])("'%s' survives write then read", (value) => {
		const written = converter.write(value);

		const read = converter.read(written);

		expect(read).toBe(value);
	});
});
