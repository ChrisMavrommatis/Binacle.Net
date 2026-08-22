import type {Alpine as AlpineType} from "alpinejs";

import {Logger, loggerPlugin} from "../../src/core/logger";

function fakeConsole() {
	return {
		info: jest.fn(),
		log: jest.fn(),
		warn: jest.fn(),
		error: jest.fn(),
	} as unknown as Console & {info: jest.Mock; log: jest.Mock; warn: jest.Mock; error: jest.Mock};
}

describe("an enabled logger", () => {
	test("info reaches the console with every argument", () => {
		const console = fakeConsole();
		const logger = new Logger(true, console);

		logger.info("[Binacle] hello", 1, {a: 2});

		expect(console.info).toHaveBeenCalledWith("[Binacle] hello", 1, {a: 2});
	});

	test("log reaches the console", () => {
		const console = fakeConsole();
		const logger = new Logger(true, console);

		logger.log("a message");

		expect(console.log).toHaveBeenCalledWith("a message");
	});

	test("warn reaches the console", () => {
		const console = fakeConsole();
		const logger = new Logger(true, console);

		logger.warn("a warning");

		expect(console.warn).toHaveBeenCalledWith("a warning");
	});

	test("error reaches the console", () => {
		const console = fakeConsole();
		const logger = new Logger(true, console);

		logger.error("a failure");

		expect(console.error).toHaveBeenCalledWith("a failure");
	});
});

describe("a disabled logger", () => {
	test("info is swallowed", () => {
		const console = fakeConsole();
		const logger = new Logger(false, console);

		logger.info("nothing");

		expect(console.info).not.toHaveBeenCalled();
	});

	test("log is swallowed", () => {
		const console = fakeConsole();
		const logger = new Logger(false, console);

		logger.log("nothing");

		expect(console.log).not.toHaveBeenCalled();
	});

	test("warn is swallowed", () => {
		const console = fakeConsole();
		const logger = new Logger(false, console);

		logger.warn("nothing");

		expect(console.warn).not.toHaveBeenCalled();
	});

	test("error is swallowed", () => {
		const console = fakeConsole();
		const logger = new Logger(false, console);

		logger.error("nothing");

		expect(console.error).not.toHaveBeenCalled();
	});
});

describe("the plugin", () => {
	test("registers the logger magic", () => {
		const magics: Record<string, unknown> = {};
		const alpine = {magic: (name: string, callback: unknown) => {magics[name] = callback;}} as unknown as AlpineType;

		loggerPlugin(alpine);

		expect(Object.keys(magics)).toEqual(["logger"]);
	});

	test("the magic hands back a logger", () => {
		const magics: Record<string, (el: unknown, context: unknown) => unknown> = {};
		const alpine = {magic: (name: string, callback: any) => {magics[name] = callback;}} as unknown as AlpineType;
		loggerPlugin(alpine);

		const logger = magics["logger"](document.createElement("div"), {Alpine: alpine});

		expect(logger).toBeInstanceOf(Logger);
	});

	test("the logger the magic hands back is enabled", () => {
		const magics: Record<string, (el: unknown, context: unknown) => unknown> = {};
		const alpine = {magic: (name: string, callback: any) => {magics[name] = callback;}} as unknown as AlpineType;
		loggerPlugin(alpine);
		const logger = magics["logger"](document.createElement("div"), {Alpine: alpine}) as Logger;
		const spy = jest.spyOn(globalThis.console, "info").mockImplementation(() => {});

		logger.info("[Binacle] enabled");

		expect(spy).toHaveBeenCalledWith("[Binacle] enabled");
		spy.mockRestore();
	});
});
