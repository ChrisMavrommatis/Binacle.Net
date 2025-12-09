import type { AlpineComponent } from 'alpinejs';

/**
 * Helper function to define Alpine.js components with proper TypeScript typing
 * @param fn - A function that returns an Alpine component
 * @returns The same function with proper typing
 */
export const defineComponent = <T>(
	fn: () => AlpineComponent<T>
): (() => AlpineComponent<T>) => fn;
