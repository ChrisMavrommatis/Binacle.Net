import { Logger } from '../core/logger';
import type { Alpine as AlpineType } from 'alpinejs';

declare module 'alpinejs' {
	interface Magics<T> {
		$logger: Logger;
	}
}

export { AlpineType };

