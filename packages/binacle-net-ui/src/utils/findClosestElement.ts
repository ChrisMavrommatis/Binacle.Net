import { ElementWithXAttributes } from "alpinejs";

export function findClosestElement(
	el: ElementWithXAttributes<HTMLElement>,
	callback: (el: ElementWithXAttributes<HTMLElement>) => boolean
): ElementWithXAttributes<HTMLElement> | null {
	if (!el) {
		return null;
	}

	if (callback(el)) {
		return el;
	}

	if (!el.parentElement) {
		return null;
	}
	return findClosestElement(el.parentElement, callback);
}
