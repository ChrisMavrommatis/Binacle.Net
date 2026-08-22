import type {Alpine as AlpineType} from "alpinejs";

import {errorsDialog, errorsDialogPlugin} from "../../src/core/errorsDialog";
import {Error as ErrorViewModel} from "../../src/viewModels";

const defaultTitle = "Something went wrong";

function createDialog() {
	return errorsDialog(defaultTitle);
}

describe("the starting state", () => {
	test("the title is the default", () => {
		const dialog = createDialog();

		const title = dialog.title;

		expect(title).toBe(defaultTitle);
	});

	test("there are no errors", () => {
		const dialog = createDialog();

		const hasErrors = dialog.hasErrors();

		expect(hasErrors).toBe(false);
	});
});

describe("a string array", () => {
	test("becomes the error list", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first", "second"]);

		expect(dialog.errors).toEqual(["first", "second"]);
	});

	test("leaves the title alone", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first"]);

		expect(dialog.title).toBe(defaultTitle);
	});

	test("makes the dialog report errors", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred(["first"]);

		expect(dialog.hasErrors()).toBe(true);
	});

	test("an empty array leaves the dialog with nothing to show", () => {
		const dialog = createDialog();

		dialog.onErrorOccurred([]);

		expect(dialog.hasErrors()).toBe(false);
	});
});

describe("an error view model", () => {
	test("its title replaces the default", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "Error: Bad Request", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.title).toBe("Error: Bad Request");
	});

	test("its errors become the error list", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "Error: Bad Request", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.errors).toEqual(["Bins is required"]);
	});

	test("an empty title leaves the default in place", () => {
		const dialog = createDialog();
		const error: ErrorViewModel = {title: "", errors: ["Bins is required"]};

		dialog.onErrorOccurred(error);

		expect(dialog.title).toBe(defaultTitle);
	});

	test("an empty error list clears whatever was showing", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred(["stale"]);

		dialog.onErrorOccurred({title: "Error", errors: []} as ErrorViewModel);

		expect(dialog.errors).toEqual([]);
	});
});

describe("closing", () => {
	test("clears the errors", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred({title: "Error: Bad Request", errors: ["Bins is required"]} as ErrorViewModel);

		dialog.closeDialog();

		expect(dialog.errors).toEqual([]);
	});

	test("puts the default title back", () => {
		const dialog = createDialog();
		dialog.onErrorOccurred({title: "Error: Bad Request", errors: ["Bins is required"]} as ErrorViewModel);

		dialog.closeDialog();

		expect(dialog.title).toBe(defaultTitle);
	});
});

describe("the plugin", () => {
	test("registers the factory under its x-data name", () => {
		const registered: Record<string, unknown> = {};
		const alpine = {data: (name: string, factory: unknown) => {registered[name] = factory;}} as unknown as AlpineType;

		errorsDialogPlugin(alpine);

		expect(registered).toEqual({errors_dialog: errorsDialog});
	});
});
