// ports C#: EncodeInvalidTests
// (bin, items) inputs the serializer must reject end-to-end. serialize is async, so use rejects.toThrow().
import ViPaqSerializer from "../src/ViPaqSerializer";
import {encodeInvalidCases} from "./providers/EncodeInvalid";

describe("serialize rejects invalid input", () => {
	test.each(encodeInvalidCases)("$name", async ({bin, items}) => {
		await expect(ViPaqSerializer.serialize(bin, items)).rejects.toThrow();
	});
});
