import {ElementWithXAttributes} from "alpinejs";

import {findClosestElement} from "../../src/utils/findClosestElement";

type XElement = ElementWithXAttributes<HTMLElement>;

function buildTree(): {root: XElement; middle: XElement; leaf: XElement} {
	const root = document.createElement("div") as XElement;
	const middle = document.createElement("section") as XElement;
	const leaf = document.createElement("input") as XElement;

	root.appendChild(middle);
	middle.appendChild(leaf);
	document.body.appendChild(root);

	return {root, middle, leaf};
}

beforeEach(() => {
	document.body.innerHTML = "";
});

test("the element itself matches before any ancestor", () => {
	const {leaf} = buildTree();

	const found = findClosestElement(leaf, () => true);

	expect(found).toBe(leaf);
});

test("the walk stops at the nearest matching ancestor", () => {
	const {middle, leaf} = buildTree();

	const found = findClosestElement(leaf, element => element.tagName === "SECTION");

	expect(found).toBe(middle);
});

test("the walk reaches the top of the tree", () => {
	const {root, leaf} = buildTree();

	const found = findClosestElement(leaf, element => element.tagName === "DIV");

	expect(found).toBe(root);
});

test("nothing matching anywhere up the tree returns null", () => {
	const {leaf} = buildTree();

	const found = findClosestElement(leaf, element => element.tagName === "TABLE");

	expect(found).toBeNull();
});

test("a detached element with no parent returns null", () => {
	const orphan = document.createElement("span") as XElement;

	const found = findClosestElement(orphan, () => false);

	expect(found).toBeNull();
});

test("a null element returns null", () => {
	const missing = null as unknown as XElement;

	const found = findClosestElement(missing, () => true);

	expect(found).toBeNull();
});
