import type {AlpineComponent} from 'alpinejs';

// Types an Alpine.js component factory without changing it.
export const defineComponent = <P extends any[], T>(
	fn: (...params: P) => AlpineComponent<T>
): ((...params: P) => AlpineComponent<T>) => fn;
