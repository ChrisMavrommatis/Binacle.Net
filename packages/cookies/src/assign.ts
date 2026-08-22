// `for...in`, not Object.assign: inherited enumerable keys are copied too.
export default function assign<T extends object>(target: T, ...sources: Array<Partial<T> | undefined>): T {
	const merged = target as Record<string, unknown>
	for (const source of sources) {
		for (const key in source) {
			merged[key] = (source as Record<string, unknown>)[key]
		}
	}
	return target
}
