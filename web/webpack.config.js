const path = require('path');
const dest = 'js';

module.exports = {
	mode: 'production',
	entry: {
		main: {
			import: './_js/main.js',
		},
		packing_visualizer: {
			import: './_js/components/packing_visualizer.js',
			dependOn: ['three']
		},
		packing_demo: {
			import: './_js/packing_demo.js',
			dependOn: ['packing_visualizer', 'alpine']
		},
		protocol_decoder: {
			import: './_js/protocol_decoder.js',
			dependOn: ['packing_visualizer', 'alpine']
		},
		three: 'three',
		alpine: 'alpinejs',
	},
	output: {
		filename: '[name].js',
		path: path.resolve(__dirname, dest),
		clean: true,
	},
	module: {
		rules: [],
	},
	plugins: [],
	optimization: {
		minimize: false,
		runtimeChunk: 'single',
	}
}
