import ControlsManager from "../../src/core/controlsManager";
import Control from "../../src/viewModels/control";
import {SceneData} from "../../src/models";

function noop() {
}

// Arrange only. The five controls in the order the manager names them.
function newManager() {
	return new ControlsManager(
		new Control(0, "first", "icon", "classes", noop),
		new Control(1, "previous", "icon", "classes", noop),
		new Control(2, "repeat", "icon", "classes", noop),
		new Control(3, "next", "icon", "classes", noop),
		new Control(4, "last", "icon", "classes", noop)
	);
}

// One object per assert, so a wiring mistake reads as a named mismatch rather than a boolean.
function enabledState(manager: ControlsManager) {
	return {
		first: manager.first.enabled,
		previous: manager.previous.enabled,
		repeat: manager.repeat.enabled,
		next: manager.next.enabled,
		last: manager.last.enabled,
	};
}

function sceneWith(itemCount: number): SceneData {
	return {
		bin: {length: 10, width: 10, height: 10},
		items: Array.from({length: itemCount}, () => ({length: 1, width: 1, height: 1, x: 0, y: 0, z: 0})),
	};
}

describe("all", () => {
	test("lists the five controls in navigation order", () => {
		const manager = newManager();

		const controls = manager.all();

		expect(controls.map(control => control.id)).toEqual(["first", "previous", "repeat", "next", "last"]);
	});
});

describe("enableAll and disableAll", () => {
	test("enableAll turns every control on", () => {
		const manager = newManager();

		manager.enableAll();

		expect(enabledState(manager)).toEqual({first: true, previous: true, repeat: true, next: true, last: true});
	});

	test("disableAll turns every control off", () => {
		const manager = newManager();
		manager.enableAll();

		manager.disableAll();

		expect(enabledState(manager)).toEqual({
			first: false,
			previous: false,
			repeat: false,
			next: false,
			last: false,
		});
	});
});

describe("updateStatus", () => {
	test("no bin disables everything", () => {
		const manager = newManager();
		manager.enableAll();

		manager.updateStatus({bin: null, items: []}, 0);

		expect(enabledState(manager)).toEqual({
			first: false,
			previous: false,
			repeat: false,
			next: false,
			last: false,
		});
	});

	test("an empty item list disables everything", () => {
		const manager = newManager();
		manager.enableAll();

		manager.updateStatus(sceneWith(0), 0);

		expect(enabledState(manager)).toEqual({
			first: false,
			previous: false,
			repeat: false,
			next: false,
			last: false,
		});
	});

	test("nothing rendered yet leaves only the forward controls and repeat", () => {
		const manager = newManager();

		manager.updateStatus(sceneWith(3), 0);

		expect(enabledState(manager)).toEqual({first: false, previous: false, repeat: true, next: true, last: true});
	});

	test("everything rendered leaves only the backward controls and repeat", () => {
		const manager = newManager();

		manager.updateStatus(sceneWith(3), 3);

		expect(enabledState(manager)).toEqual({first: true, previous: true, repeat: true, next: false, last: false});
	});

	test("more rendered than there are items counts as the end", () => {
		const manager = newManager();

		manager.updateStatus(sceneWith(3), 4);

		expect(enabledState(manager)).toEqual({first: true, previous: true, repeat: true, next: false, last: false});
	});

	test("part way through enables everything", () => {
		const manager = newManager();

		manager.updateStatus(sceneWith(3), 1);

		expect(enabledState(manager)).toEqual({first: true, previous: true, repeat: true, next: true, last: true});
	});

	test("a single item goes straight from nothing rendered to the end", () => {
		const manager = newManager();

		manager.updateStatus(sceneWith(1), 1);

		expect(enabledState(manager)).toEqual({first: true, previous: true, repeat: true, next: false, last: false});
	});
});
