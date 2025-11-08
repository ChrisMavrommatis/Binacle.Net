import Alpine from 'alpinejs';
import packingdemoapp from './components/packing-demo-app.js';
import packingVisualizer from "./components/packing-visualizer";


Alpine.data('packingdemoapp', packingdemoapp);
Alpine.data('packingvisualizer', packingVisualizer);

Alpine.start();
