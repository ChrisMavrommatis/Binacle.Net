import type {AlpineComponent} from 'alpinejs';

/**
 * Helper function to define Alpine.js components with proper TypeScript typing
 * @param fn - A function that returns an Alpine component
 * @returns The same function with proper typing
 */
export const defineComponent = <P extends any[], T>(
	fn: (...params: P) => AlpineComponent<T>
): ((...params: P) => AlpineComponent<T>) => fn;
