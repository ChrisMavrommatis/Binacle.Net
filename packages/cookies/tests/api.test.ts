import Cookies from "../src/api";

// jsdom keeps a real cookie jar, so these are round trips through document.cookie rather than a stub.
// The jar is per test file and does not reset itself; clearCookies below is what keeps tests independent.
function clearCookies(): void {
	for (const cookie of document.cookie.split("; ")) {
		const name = cookie.split("=")[0];
		if (name) {
			document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
		}
	}
}

beforeEach(() => {
	clearCookies();
});

afterEach(() => {
	jest.useRealTimers();
});

describe("set and get", () => {
	test("a value written by set reads back", () => {
		const name = "flavour";

		Cookies.set(name, "chocolate");

		expect(Cookies.get(name)).toBe("chocolate");
	});

	test("a value holding cookie separators survives the round trip", () => {
		const value = "hello world; and, more";

		Cookies.set("greeting", value);

		expect(Cookies.get("greeting")).toBe(value);
	});

	test("a name holding a space survives the round trip", () => {
		const name = "two words";

		Cookies.set(name, "1");

		expect(Cookies.get(name)).toBe("1");
	});

	test("a cookie written straight to document.cookie is decoded on the way out", () => {
		document.cookie = "raw=plain%20value; path=/";

		const value = Cookies.get("raw");

		expect(value).toBe("plain value");
	});

	test("a missing cookie reads as undefined", () => {
		Cookies.set("present", "1");

		const value = Cookies.get("absent");

		expect(value).toBeUndefined();
	});

	test("get with no argument returns every cookie", () => {
		Cookies.set("first", "1");
		Cookies.set("second", "2");

		const jar = Cookies.get();

		expect(jar).toEqual({first: "1", second: "2"});
	});

	test("get with an empty name returns undefined, not the jar", () => {
		Cookies.set("first", "1");

		const value = Cookies.get("");

		expect(value).toBeUndefined();
	});
});

describe("remove", () => {
	test("a removed cookie is gone", () => {
		Cookies.set("temporary", "1");

		Cookies.remove("temporary");

		expect(Cookies.get("temporary")).toBeUndefined();
	});

	test("removing one cookie leaves the others", () => {
		Cookies.set("keep", "1");
		Cookies.set("drop", "2");

		Cookies.remove("drop");

		expect(Cookies.get()).toEqual({keep: "1"});
	});

	// A cookie is identified by its path as well as its name, so the attributes have to match the ones it was
	// written with. Without them the removal writes an expired cookie on a different path and reports nothing.
	test("a cookie written on a path needs that path to remove it", () => {
		Cookies.set("scoped", "1", {path: "/"});

		Cookies.remove("scoped", {path: "/"});

		expect(Cookies.get("scoped")).toBeUndefined();
	});

	test("the removal expires the cookie rather than trusting the caller's expiry", () => {
		Cookies.set("scoped", "1");

		Cookies.remove("scoped", {expires: 365});

		expect(Cookies.get("scoped")).toBeUndefined();
	});
});

describe("attributes", () => {
	test("the defaults are written in declaration order, with expires as days from now", () => {
		jest.useFakeTimers().setSystemTime(new Date("2030-01-01T00:00:00.000Z"));

		const written = Cookies.set("a", "1");

		expect(written).toBe("a=1; path=/; expires=Mon, 01 Apr 2030 00:00:00 GMT; sameSite=Lax; secure");
	});

	test("a numeric expires is read as a count of days", () => {
		jest.useFakeTimers().setSystemTime(new Date("2030-01-01T00:00:00.000Z"));

		const written = Cookies.set("a", "1", {expires: 365});

		expect(written).toContain("; expires=Wed, 01 Jan 2031 00:00:00 GMT");
	});

	test("a Date expires is used as given", () => {
		const expires = new Date("2031-06-15T12:00:00.000Z");

		const written = Cookies.set("a", "1", {expires});

		expect(written).toContain("; expires=Sun, 15 Jun 2031 12:00:00 GMT");
	});

	test("path overrides the default without moving position", () => {
		jest.useFakeTimers().setSystemTime(new Date("2030-01-01T00:00:00.000Z"));

		const written = Cookies.set("a", "1", {path: "/demo"});

		expect(written).toBe("a=1; path=/demo; expires=Mon, 01 Apr 2030 00:00:00 GMT; sameSite=Lax; secure");
	});

	test("an attribute value is cut at the first semicolon", () => {
		const written = Cookies.set("a", "1", {path: "/one;/two"});

		expect(written).toContain("; path=/one; expires=");
	});

	test("a falsy attribute is left out entirely", () => {
		jest.useFakeTimers().setSystemTime(new Date("2030-01-01T00:00:00.000Z"));

		const written = Cookies.set("a", "1", {secure: false, sameSite: ""});

		expect(written).toBe("a=1; path=/; expires=Mon, 01 Apr 2030 00:00:00 GMT");
	});

	test("a boolean true attribute is written as a bare flag", () => {
		const written = Cookies.set("a", "1");

		expect(written).toMatch(/; secure$/);
	});
});
