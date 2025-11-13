import Alpine from 'alpinejs';
import packing_demo_app from './components/packing_demo_app';
import packing_visualizer from './components/packing_visualizer';


import fieldPlugin from './plugins/field.js';
import loggerPlugin from './plugins/logger';

Alpine.plugin(fieldPlugin)
Alpine.plugin(loggerPlugin);

Alpine.data('packing_demo_app', (base_url) => packing_demo_app(base_url));
Alpine.data('packing_visualizer', packing_visualizer);

Alpine.start();

