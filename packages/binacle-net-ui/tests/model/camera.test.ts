import {cameraFar} from "../../src/utils/cameraFar";
import {cameraFov} from "../../src/utils/cameraFov";
import {getCameraPosition} from "../../src/utils/getCameraPosition";

describe("cameraFar", () => {
	test("falls back to a thousand when there is no bin", () => {
		const bin = null;

		const far = cameraFar(bin);

		expect(far).toBe(1000);
	});

	// Twice the corner-to-corner distance plus twice the height, rounded up.
	test("is twice the diagonal plus twice the height, rounded up", () => {
		const bin = {length: 10, width: 20, height: 30};

		const far = cameraFar(bin);

		expect(far).toBe(135);
	});

	test("grows with the bin", () => {
		const small = {length: 10, width: 10, height: 10};
		const large = {length: 100, width: 100, height: 100};

		const fars = [cameraFar(small), cameraFar(large)];

		expect(fars[0]).toBeLessThan(fars[1]);
	});
});

describe("cameraFov", () => {
	test.each([
		[0.3, 65],
		[0.59, 65],
		[0.6, 50],
		[0.99, 50],
		[1, 40],
		[2.5, 40],
	])("an aspect ratio of %f gives %i degrees", (aspectRatio, expected) => {
		const fov = cameraFov(aspectRatio);

		expect(fov).toBe(expected);
	});
});

describe("getCameraPosition", () => {
	test.each([
		["length", {length: 30, width: 10, height: 20}],
		["height", {length: 10, width: 20, height: 30}],
		["width", {length: 10, width: 30, height: 20}],
	])("takes the longest side, whichever is %s", (_name, bin) => {
		const position = getCameraPosition(bin as {length: number; width: number; height: number});

		expect(position).toEqual({x: 30, y: 30, z: 36});
	});

	test("pulls further back for a cube", () => {
		const bin = {length: 10, width: 10, height: 10};

		const position = getCameraPosition(bin);

		expect(position).toEqual({x: 10, y: 10, z: 20});
	});
});
