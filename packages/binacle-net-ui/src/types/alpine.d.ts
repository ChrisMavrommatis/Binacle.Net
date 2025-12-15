import { Logger } from '../core';
import type { Alpine as AlpineType } from 'alpinejs';

declare module 'alpinejs' {
	interface Magics<T> {
		$logger: Logger;
	}

	interface XAttributes {
		_x_fieldPrefix?: string;
	}

}

export { AlpineType };
