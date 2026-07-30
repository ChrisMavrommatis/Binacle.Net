// SHAPE 1 consumer.  ports C#: DecodeInvalidTests
// The test file is pure assertions — the parsing lives in providers/decodeInvalid.ts.

import ViPaqSerializer from "../src/ViPaqSerializer";
import {decodeInvalidCases} from "./providers/DecodeInvalid";

describe("decode rejects invalid blobs", () => {
	test.each(decodeInvalidCases)("$name", async ({blob}) => {
		await expect(ViPaqSerializer.deserialize(new Uint8Array(blob))).rejects.toThrow();
	});
});
