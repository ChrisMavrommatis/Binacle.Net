import Alpine from 'alpinejs';
import protocol_decoder_app from './components/protocol_decoder_app';
import packing_visualizer from './components/packing_visualizer';
import errors_dialog from "./components/errors_dialog";

import loggerPlugin from './plugins/logger';

Alpine.plugin(loggerPlugin);

Alpine.data('protocol_decoder_app',  protocol_decoder_app);
Alpine.data('packing_visualizer', packing_visualizer);
Alpine.data('errors_dialog', (default_title) => errors_dialog(default_title));

Alpine.start();
