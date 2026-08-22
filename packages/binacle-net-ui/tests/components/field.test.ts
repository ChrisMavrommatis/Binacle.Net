import type {Alpine as AlpineType, ElementWithXAttributes} from "alpinejs";

import {fieldPlugin} from "../../src/core/field";

type XElement = ElementWithXAttributes<HTMLElement>;
type NameFn = (fieldName: string, fieldIndex: number | null) => string;

interface Field {
	applyPrefix: (el: HTMLElement, prefix: string) => void;
	fieldId: (el: HTMLElement) => NameFn;
	fieldName: (el: HTMLElement) => NameFn;
}

function registerField(): Field {
	const directives: Record<string, any> = {};
	const magics: Record<string, any> = {};
	const alpine = {
		directive: (name: string, callback: any) => {directives[name] = callback;},
		magic: (name: string, callback: any) => {magics[name] = callback;},
	} as unknown as AlpineType;

	fieldPlugin(alpine);

	return {
		applyPrefix: (el, prefix) =>
			directives["field-prefix"](el, {value: "", modifiers: [], expression: prefix}, {evaluate: () => prefix}),
		fieldId: el => magics["fieldId"](el, {Alpine: alpine}) as NameFn,
		fieldName: el => magics["fieldName"](el, {Alpine: alpine}) as NameFn,
	};
}

// A field input inside an optional prefixed wrapper, both in the document so the magics can walk up.
function buildField(field: Field, prefix: string | null): HTMLElement {
	const wrapper = document.createElement("div");
	const input = document.createElement("input");

	wrapper.appendChild(input);
	document.body.appendChild(wrapper);
	if (prefix !== null) {
		field.applyPrefix(wrapper, prefix);
	}

	return input;
}

beforeEach(() => {
	document.body.innerHTML = "";
});

// The whole naming table. Ids join with underscores, names use the bracket-and-dot form a model binder reads.
const namingRules = [
	{prefix: null, index: null, id: "length", name: "length"},
	{prefix: null, index: 0, id: "length_0", name: "length[0]"},
	{prefix: "bins", index: null, id: "bins_length", name: "bins.length"},
	{prefix: "bins", index: 2, id: "bins_2_length", name: "bins[2].length"},
];

describe("the naming table", () => {
	test.each(namingRules)("prefix $prefix index $index gives the id $id", ({prefix, index, id}) => {
		const field = registerField();
		const input = buildField(field, prefix);

		const result = field.fieldId(input)("length", index);

		expect(result).toBe(id);
	});

	test.each(namingRules)("prefix $prefix index $index gives the name $name", ({prefix, index, name}) => {
		const field = registerField();
		const input = buildField(field, prefix);

		const result = field.fieldName(input)("length", index);

		expect(result).toBe(name);
	});
});

describe("x-field-prefix", () => {
	test("stores the expression on the element", () => {
		const field = registerField();
		const wrapper = document.createElement("div") as XElement;

		field.applyPrefix(wrapper, "items");

		expect(wrapper._x_fieldPrefix).toBe("items");
	});

	test("an element carrying the prefix itself uses it", () => {
		const field = registerField();
		const wrapper = document.createElement("div");
		document.body.appendChild(wrapper);
		field.applyPrefix(wrapper, "items");

		const result = field.fieldName(wrapper)("width", null);

		expect(result).toBe("items.width");
	});

	test("the nearest prefix wins over an outer one", () => {
		const field = registerField();
		const outer = document.createElement("div");
		const inner = document.createElement("div");
		const input = document.createElement("input");
		inner.appendChild(input);
		outer.appendChild(inner);
		document.body.appendChild(outer);
		field.applyPrefix(outer, "bins");
		field.applyPrefix(inner, "items");

		const result = field.fieldName(input)("height", 1);

		expect(result).toBe("items[1].height");
	});

	test("an empty prefix is treated as no prefix", () => {
		const field = registerField();
		const input = buildField(field, "");

		const result = field.fieldName(input)("height", 3);

		expect(result).toBe("height[3]");
	});
});
