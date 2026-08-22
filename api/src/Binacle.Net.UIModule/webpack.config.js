const path = require('path');

module.exports = (env) => {
	const production = (env.build || 'dist') === 'dist';

	return {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.js',
			instance: './_js/instance.js',
			packing_demo: './_js/packing_demo.js',
			protocol_decoder: './_js/protocol_decoder.js'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, 'wwwroot/js'),
			// Not in watch mode: a running Kestrel has already listed these files.
			clean: production,
		},
		resolve: {
			extensions: ['.ts', '.js', '.json'],
		},
		module: {
			rules: [
				{
					test: /\.ts$/,
					use: 'ts-loader',
					exclude: /node_modules/,
				},
			],
		},
		optimization: {
			minimize: production,
			runtimeChunk: 'single',
			splitChunks: {
				chunks: 'all',
				automaticNameDelimiter: '-',
				minSize: 0,
				cacheGroups: {
					// three ships as 3 pre-bundled modules, 566 KiB minified, no tree shaking.
					three: {
						test: /[\\/]node_modules[\\/]three[\\/]/,
						name: 'three',
						chunks: 'all',
						enforce: true,
						priority: 30,
					},
					vendors: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendors',
						chunks: 'all',
						enforce: true,
						priority: 10,
					},
					binacleNetUi: {
						test: /[\\/]packages[\\/]binacle-net-ui[\\/]/,
						name: 'binacle-net-ui',
						chunks: 'all',
						enforce: true,
						priority: 20,
					},
					binacleViPaq: {
						test: /[\\/]vipaq[\\/]packages[\\/]binacle-vipaq[\\/]/,
						name: 'binacle-vipaq',
						chunks: 'all',
						enforce: true,
						priority: 20,
					},
				},
			},
		},
		performance: {
			hints: production ? 'warning' : false,
			maxAssetSize: 300 * 1024,
			maxEntrypointSize: 300 * 1024,
			// three is unavoidable on the demo pages. Budget everything else.
			assetFilter: (asset) => asset.endsWith('.js') && !asset.startsWith('three'),
		},
		// A warm cache reports success on source that fails a cold build, type errors included. Delete
		// node_modules/.cache/webpack before trusting a clean run.
		cache: {type: 'filesystem'},
		devtool: production ? false : 'source-map',
	};
};
