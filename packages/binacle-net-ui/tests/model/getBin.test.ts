import {BoxGeometry, EdgesGeometry, LineSegments, Scene} from "three";
import {getBin} from "../../src/utils/getBin";

// createBin draws the bin as edges over a box, with length on x, height on y and width on z. getBin reads
// that back, so the test builds the same shape rather than calling into the scene helpers.
function sceneWithBin(length: number, width: number, height: number) {
	const scene = new Scene();
	const edges = new LineSegments(new EdgesGeometry(new BoxGeometry(length, height, width)));
	edges.name = "bin";
	scene.add(edges);
	return scene;
}

test("reads the bin sides back off the scene", () => {
	const scene = sceneWithBin(10, 20, 30);

	const bin = getBin(scene);

	expect(bin).toEqual({length: 10, width: 20, height: 30});
});

test("is null when the scene has no bin", () => {
	const scene = new Scene();

	const bin = getBin(scene);

	expect(bin).toBeNull();
});

test("ignores objects with another name", () => {
	const scene = new Scene();
	const edges = new LineSegments(new EdgesGeometry(new BoxGeometry(10, 20, 30)));
	edges.name = "item";
	scene.add(edges);

	const bin = getBin(scene);

	expect(bin).toBeNull();
});
