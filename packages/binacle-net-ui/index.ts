import Alpine from 'alpinejs';
import { registerLogger } from './src/core/logger';

// Register plugins
Alpine.plugin(registerLogger);

// Export components and utilities for use in other modules
export { Logger } from './src/core/logger';
export { defineComponent } from './src/utils/defineComponent';
export * from './src/models';
export * from './src/components';
export * from './src/utils';

// Start Alpine
Alpine.start();
