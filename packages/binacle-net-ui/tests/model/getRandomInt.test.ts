import {getRandomInt} from "../../src/utils/getRandomInt";

afterEach(() => {
	jest.restoreAllMocks();
});

test("stays inside the range over many rolls", () => {
	const rolls = 10000;

	const values = Array.from({length: rolls}, () => getRandomInt(3, 7));

	expect(values.filter(value => !Number.isInteger(value) || value < 3 || value > 7)).toEqual([]);
});

test("reaches both ends of the range", () => {
	const rolls = 10000;

	const values = Array.from({length: rolls}, () => getRandomInt(3, 7));

	expect(new Set(values)).toEqual(new Set([3, 4, 5, 6, 7]));
});

test("a single-value range always returns that value", () => {
	const rolls = 100;

	const values = Array.from({length: rolls}, () => getRandomInt(5, 5));

	expect(new Set(values)).toEqual(new Set([5]));
});

test("the lowest roll gives the minimum", () => {
	jest.spyOn(Math, "random").mockReturnValue(0);

	const value = getRandomInt(3, 7);

	expect(value).toBe(3);
});

test("the highest roll gives the maximum", () => {
	jest.spyOn(Math, "random").mockReturnValue(0.999999);

	const value = getRandomInt(3, 7);

	expect(value).toBe(7);
});

// Both bounds are pulled inwards, so the range is the whole numbers the caller's range contains.
test("fractional bounds round inwards", () => {
	jest.spyOn(Math, "random").mockReturnValue(0);

	const value = getRandomInt(1.2, 3.8);

	expect(value).toBe(2);
});

test("fractional bounds cannot exceed the floor of the maximum", () => {
	jest.spyOn(Math, "random").mockReturnValue(0.999999);

	const value = getRandomInt(1.2, 3.8);

	expect(value).toBe(3);
});
