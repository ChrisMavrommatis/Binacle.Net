import Alpine from 'alpinejs';
import packingDemoForm from './components/packing-demo-form.js';
import packingDemoApp from './components/packing-demo-app.js';
import packingVisualizer from "./components/packing-visualizer";


import fieldPlugin from './plugins/field.js';

Alpine.plugin(fieldPlugin)

Alpine.data('packingdemoapp', (base_url) => packingDemoApp(base_url));
Alpine.data('packingdemoform', packingDemoForm);
Alpine.data('packingvisualizer', packingVisualizer);

Alpine.start();

