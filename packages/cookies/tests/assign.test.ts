import assign from "../src/assign";

describe("assign", () => {
	test("a later source wins", () => {
		const target: Record<string, string> = {path: "/"};

		const merged = assign(target, {path: "/demo"});

		expect(merged.path).toBe("/demo");
	});

	test("an undefined source is skipped", () => {
		const target: Record<string, string> = {path: "/"};

		const merged = assign(target, undefined);

		expect(merged).toEqual({path: "/"});
	});

	test("inherited enumerable keys are copied too", () => {
		const source = Object.create({path: "/inherited"}) as Record<string, string>;

		const merged = assign({} as Record<string, string>, source);

		expect(merged.path).toBe("/inherited");
	});

	test("the target itself is returned, not a copy", () => {
		const target: Record<string, string> = {};

		const merged = assign(target, {path: "/"});

		expect(merged).toBe(target);
	});
});
