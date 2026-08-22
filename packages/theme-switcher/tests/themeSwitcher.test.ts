import {Cookies} from "cookies";

import ThemeSwitcherButtonElement from "../src/themeSwitcher";

// jsdom gives us a real customElements registry, so connectedCallback fires on appendChild. The tag can
// only be defined once per file - a second define throws.
beforeAll(() => {
	customElements.define("theme-switcher", ThemeSwitcherButtonElement);
});

function clearCookies(): void {
	for (const cookie of document.cookie.split("; ")) {
		const name = cookie.split("=")[0];
		if (name) {
			document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
		}
	}
}

function createSwitcher(defaultTheme?: string): ThemeSwitcherButtonElement {
	const element = document.createElement("theme-switcher") as ThemeSwitcherButtonElement;
	if (defaultTheme) {
		element.dataset.defaultTheme = defaultTheme;
	}
	return element;
}

beforeEach(() => {
	clearCookies();
	document.body.innerHTML = "";
	document.body.className = "";
});

describe("picking the theme on connect", () => {
	test("data-default-theme of dark puts the body in dark", () => {
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(document.body.classList.contains("dark")).toBe(true);
	});

	test("data-default-theme of dark takes the body out of light", () => {
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(document.body.classList.contains("light")).toBe(false);
	});

	test("data-default-theme of light puts the body in light", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(document.body.classList.contains("light")).toBe(true);
	});

	test("no data-default-theme falls back to light", () => {
		const element = createSwitcher();

		document.body.appendChild(element);

		expect(document.body.classList.contains("light")).toBe(true);
	});

	test("an existing theme cookie beats the default", () => {
		Cookies.set("theme", "light");
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(document.body.classList.contains("light")).toBe(true);
	});

	test("an existing theme cookie is not overwritten on connect", () => {
		Cookies.set("theme", "light");
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(Cookies.get("theme")).toBe("light");
	});

	test("the default theme is not written to a cookie", () => {
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(Cookies.get("theme")).toBeUndefined();
	});
});

describe("clicking", () => {
	test("light becomes dark on the body", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.click();

		expect(document.body.classList.contains("dark")).toBe(true);
	});

	test("light is taken off the body", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.click();

		expect(document.body.classList.contains("light")).toBe(false);
	});

	test("dark becomes light on the body", () => {
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		element.click();

		expect(document.body.classList.contains("light")).toBe(true);
	});

	test("the new theme is written to the cookie", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.click();

		expect(Cookies.get("theme")).toBe("dark");
	});

	test("a second click writes the theme back", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.click();
		element.click();

		expect(Cookies.get("theme")).toBe("light");
	});

	test("a themeChanged event carries the new theme", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);
		const listener = jest.fn();
		window.addEventListener("themeChanged", listener);

		element.click();

		expect(listener.mock.calls[0][0].detail).toEqual({theme: "dark"});
	});
});

describe("the icon", () => {
	test("a light body renders the icon that offers dark", () => {
		const element = createSwitcher("light");

		document.body.appendChild(element);

		expect(element.querySelector("i")?.textContent).toBe("dark_mode");
	});

	test("a dark body renders the icon that offers light", () => {
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(element.querySelector("i")?.textContent).toBe("light_mode");
	});

	test("the icon swaps when the theme is clicked", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.click();

		expect(element.querySelector("i")?.textContent).toBe("light_mode");
	});
});

describe("elements marked with data-theme", () => {
	test("a dark body gets the data-darktheme value", () => {
		document.body.innerHTML = "<img data-theme=\"src\" data-darktheme=\"/dark.png\" data-lighttheme=\"/light.png\">";
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/dark.png");
	});

	test("a click moves them to the other theme", () => {
		document.body.innerHTML = "<img data-theme=\"src\" data-darktheme=\"/dark.png\" data-lighttheme=\"/light.png\">";
		const element = createSwitcher("dark");
		document.body.appendChild(element);

		element.click();

		expect(document.querySelector("img")?.getAttribute("src")).toBe("/light.png");
	});

	// Today's behaviour, not the wanted one: a missing data-darktheme writes the string "undefined"
	// rather than being skipped.
	test("a missing theme value is written as the string undefined", () => {
		document.body.innerHTML = "<img data-theme=\"src\">";
		const element = createSwitcher("dark");

		document.body.appendChild(element);

		expect(document.querySelector("img")?.getAttribute("src")).toBe("undefined");
	});
});

// Pinned as it is today. The disconnect hook is spelled disconectedCallback - one n - so the browser never
// calls it, and the click listener outlives the element. Fixing the spelling changes what these two assert.
describe("disconnecting", () => {
	test("the browser lifecycle hook is not implemented", () => {
		const element = createSwitcher("light");

		const hook = (element as unknown as Record<string, unknown>).disconnectedCallback;

		expect(hook).toBeUndefined();
	});

	test("the click listener survives removal from the document", () => {
		const element = createSwitcher("light");
		document.body.appendChild(element);

		element.remove();
		element.click();

		expect(document.body.classList.contains("dark")).toBe(true);
	});
});
