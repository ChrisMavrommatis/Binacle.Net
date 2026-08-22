import Item from "../../src/viewModels/item";

describe("id", () => {
	test("adds the quantity to the box id", () => {
		const item = new Item(10, 20, 30, 4);

		const id = item.id;

		expect(id).toBe("10x20x30-4");
	});
});

describe("getDimensions", () => {
	test("adds quantity as a fourth dimension", () => {
		const item = new Item(10, 20, 30, 4);

		const dimensions = item.getDimensions();

		expect(dimensions).toEqual([
			{name: "Length", value: 10},
			{name: "Width", value: 20},
			{name: "Height", value: 30},
			{name: "Quantity", value: 4},
		]);
	});
});

describe("validation", () => {
	test("a valid item reports no errors", () => {
		const item = new Item(10, 20, 30, 4);

		const errors = item.allErrorMessages;

		expect(errors).toEqual([]);
	});

	test("quantity is checked by the same rules as the sides", () => {
		const item = new Item(10, 20, 30, 0);

		const errors = item.allErrorMessages;

		expect(errors).toEqual(["Quantity must be between 1 and 65535"]);
	});

	test("a fractional quantity is not an integer", () => {
		const item = new Item(10, 20, 30, 1.5);

		const errors = item.allErrorMessages;

		expect(errors).toEqual(["Quantity must be an integer"]);
	});

	test("isFieldValid answers for quantity too", () => {
		const item = new Item(10, 20, 30, 0);

		const valid = item.isFieldValid("quantity");

		expect(valid).toBe(false);
	});
});
