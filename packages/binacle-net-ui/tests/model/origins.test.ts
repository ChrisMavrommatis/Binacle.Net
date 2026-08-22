import {getBinOrigin} from "../../src/utils/getBinOrigin";
import {getItemOrigin} from "../../src/utils/getItemOrigin";
import {getMeshPosition} from "../../src/utils/getMeshPosition";

describe("getBinOrigin", () => {
	// Three.js maps length to x, height to y and width to z. The bin is centred on the origin, so its corner
	// is half of each side in the negative direction.
	test("is half of each side, negative, with width on z", () => {
		const bin = {length: 10, width: 20, height: 30};

		const origin = getBinOrigin(bin);

		expect(origin).toEqual({x: -5, y: -15, z: -10});
	});
});

describe("getItemOrigin", () => {
	test("is half of each side, positive, with width on z", () => {
		const item = {length: 10, width: 20, height: 30};

		const origin = getItemOrigin(item);

		expect(origin).toEqual({x: 5, y: 15, z: 10});
	});
});

describe("getMeshPosition", () => {
	// The packed coordinates come in with y and z the other way round from the scene, so the sum swaps them.
	test("adds the three, taking the packed z for y and the packed y for z", () => {
		const binOrigin = {x: -5, y: -15, z: -10};
		const itemOrigin = {x: 1, y: 2, z: 3};
		const packedItem = {x: 100, y: 200, z: 300};

		const position = getMeshPosition(binOrigin, itemOrigin, packedItem);

		expect(position).toEqual({x: 96, y: 287, z: 193});
	});
});
