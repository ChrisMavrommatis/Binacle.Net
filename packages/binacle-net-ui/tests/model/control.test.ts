import Control from "../../src/viewModels/control";

function noop() {
}

test("a new control starts disabled", () => {
	const control = new Control(0, "first", "icon", "classes", noop);

	const enabled = control.enabled;

	expect(enabled).toBe(false);
});

test("enable turns it on", () => {
	const control = new Control(0, "first", "icon", "classes", noop);

	control.enable();

	expect(control.enabled).toBe(true);
});

test("disable turns it back off", () => {
	const control = new Control(0, "first", "icon", "classes", noop);
	control.enable();

	control.disable();

	expect(control.enabled).toBe(false);
});

test("the click handler is kept as given", () => {
	const onClick = jest.fn();
	const control = new Control(3, "next", "icon", "classes", onClick);

	control.onClick();

	expect(onClick).toHaveBeenCalledTimes(1);
});
