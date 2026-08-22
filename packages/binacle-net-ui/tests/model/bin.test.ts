import Bin from "../../src/viewModels/bin";
import Box from "../../src/viewModels/box";

test("a bin is a box", () => {
	const bin = new Bin(10, 20, 30);

	const isBox = bin instanceof Box;

	expect(isBox).toBe(true);
});

test("a bin carries the box id", () => {
	const bin = new Bin(10, 20, 30);

	const id = bin.id;

	expect(id).toBe("10x20x30");
});

test("a bin carries the box validation", () => {
	const bin = new Bin(10, 20, 0);

	const errors = bin.allErrorMessages;

	expect(errors).toEqual(["Height must be between 1 and 65535"]);
});
