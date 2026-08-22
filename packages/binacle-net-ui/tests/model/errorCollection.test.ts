import ErrorCollection from "../../src/viewModels/errorCollection";

describe("push and hasError", () => {
	test("a field pushed in mixed case is found in lower case", () => {
		const errors = new ErrorCollection();

		errors.push("Length", "Length is required");

		expect(errors.hasError("length")).toBe(true);
	});

	test("a field pushed in lower case is found in mixed case", () => {
		const errors = new ErrorCollection();

		errors.push("length", "Length is required");

		expect(errors.hasError("LENGTH")).toBe(true);
	});

	test("two spellings of the same field share one bucket", () => {
		const errors = new ErrorCollection();

		errors.push("Length", "first");
		errors.push("LENGTH", "second");

		expect(errors.errorMessages).toEqual(["first", "second"]);
	});

	test("an untouched field is false", () => {
		const errors = new ErrorCollection();

		const result = errors.hasError("width");

		expect(result).toBe(false);
	});
});

describe("errorMessages", () => {
	test("is empty for a new collection", () => {
		const errors = new ErrorCollection();

		const messages = errors.errorMessages;

		expect(messages).toEqual([]);
	});

	test("returns every field's messages in the order the fields were first seen", () => {
		const errors = new ErrorCollection();

		errors.push("Width", "width one");
		errors.push("Length", "length one");
		errors.push("Width", "width two");

		expect(errors.errorMessages).toEqual(["width one", "width two", "length one"]);
	});
});

describe("hasErrors", () => {
	test("is false for a new collection", () => {
		const errors = new ErrorCollection();

		const result = errors.hasErrors();

		expect(result).toBe(false);
	});

	test("is true once any field has a message", () => {
		const errors = new ErrorCollection();

		errors.push("Height", "Height must be an integer");

		expect(errors.hasErrors()).toBe(true);
	});
});
