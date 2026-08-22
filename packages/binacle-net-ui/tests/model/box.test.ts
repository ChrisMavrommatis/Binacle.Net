import Box from "../../src/viewModels/box";

describe("id", () => {
	test("joins the three sides with x", () => {
		const box = new Box(10, 20, 30);

		const id = box.id;

		expect(id).toBe("10x20x30");
	});
});

describe("getDimensions", () => {
	test("names the three sides and coerces the values with Number", () => {
		const box = new Box("10" as unknown as number, 20, 30);

		const dimensions = box.getDimensions();

		expect(dimensions).toEqual([
			{name: "Length", value: 10},
			{name: "Width", value: 20},
			{name: "Height", value: 30},
		]);
	});
});

describe("a valid box", () => {
	test.each([
		["the smallest allowed side", 1],
		["a middling side", 500],
		["the largest allowed side", 65535],
	])("%s passes", (_name, side) => {
		const box = new Box(side as number, side as number, side as number);

		const errors = box.allErrorMessages;

		expect(errors).toEqual([]);
	});

	test("reports no errors", () => {
		const box = new Box(10, 20, 30);

		const hasErrors = box.hasErrors();

		expect(hasErrors).toBe(false);
	});
});

describe("range", () => {
	test.each([
		["zero", 0],
		["negative", -1],
		["one over the maximum", 65536],
	])("%s fails the range check", (_name, side) => {
		const box = new Box(side as number, 20, 30);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Length must be between 1 and 65535"]);
	});
});

describe("integer", () => {
	test("a fractional side is not an integer", () => {
		const box = new Box(10, 20.5, 30);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Width must be an integer"]);
	});

	test("a fraction below the minimum fails both checks", () => {
		const box = new Box(10, 20, 0.5);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Height must be an integer", "Height must be between 1 and 65535"]);
	});
});

describe("not a number", () => {
	test("a value Number cannot read is reported as not a number and not an integer", () => {
		const box = new Box("abc" as unknown as number, 20, 30);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Length must be a number", "Length must be an integer"]);
	});

	test("undefined becomes NaN, so it reads as not a number rather than missing", () => {
		const box = new Box(undefined as unknown as number, 20, 30);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Length must be a number", "Length must be an integer"]);
	});

	// getDimensions has already run the value through Number, so null arrives as 0 and the "is required"
	// branch in errorState can never fire. The message below is what a user actually sees for a blank field.
	test("null becomes zero, so it reads as out of range rather than missing", () => {
		const box = new Box(null as unknown as number, 20, 30);

		const errors = box.allErrorMessages;

		expect(errors).toEqual(["Length must be between 1 and 65535"]);
	});

	test("no dimension value ever reaches the required check", () => {
		const boxes = [
			new Box(null as unknown as number, null as unknown as number, null as unknown as number),
			new Box(undefined as unknown as number, undefined as unknown as number, undefined as unknown as number),
		];

		const errors = boxes.flatMap(box => box.allErrorMessages);

		expect(errors.filter(message => message.endsWith("is required"))).toEqual([]);
	});
});

describe("allErrorMessages", () => {
	test("collects every field in dimension order", () => {
		const box = new Box(0, 20.5, "abc" as unknown as number);

		const errors = box.allErrorMessages;

		expect(errors).toEqual([
			"Length must be between 1 and 65535",
			"Width must be an integer",
			"Height must be a number",
			"Height must be an integer",
		]);
	});
});

describe("isFieldValid", () => {
	test("is false for the field that failed", () => {
		const box = new Box(0, 20, 30);

		const valid = box.isFieldValid("Length");

		expect(valid).toBe(false);
	});

	test("is true for a field that passed", () => {
		const box = new Box(0, 20, 30);

		const valid = box.isFieldValid("Width");

		expect(valid).toBe(true);
	});

	test("matches the field name whatever its case", () => {
		const box = new Box(0, 20, 30);

		const valid = box.isFieldValid("length");

		expect(valid).toBe(false);
	});

	test("is true for a name no dimension uses", () => {
		const box = new Box(0, 20, 30);

		const valid = box.isFieldValid("Depth");

		expect(valid).toBe(true);
	});

	test("is true when no field name is given", () => {
		const box = new Box(0, 20, 30);

		const valid = box.isFieldValid("");

		expect(valid).toBe(true);
	});
});
