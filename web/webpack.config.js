const path = require('path');
const dest = 'js';

module.exports = (env, argv) => {
	const buildType = env.build || 'dist'; // 'dist' or 'watch'
	const production = buildType === 'dist';
	console.log(`Environment Build: ${buildType}`);

	return {
		mode: production ? 'production': 'development',
		entry: {
			main: './_js/main.js',
			packing_demo: './_js/packing_demo.js',
			protocol_decoder: './_js/protocol_decoder.js'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, dest),
			clean: true,
		},
		resolve: {
			extensions: ['.ts', '.js', '.json'],
			alias: {
				three: path.resolve(__dirname, 'node_modules/three'),
			},
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
		plugins: [],
		optimization: {
			minimize: production ? true : false,
			runtimeChunk: 'single',
			splitChunks: {
				chunks: 'all',
				automaticNameDelimiter: '-',
				minSize: 0,
				cacheGroups: {
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
						priority: 20, // higher than vendors
					},
					binacleViPaq: {
						test: /[\\/]packages[\\/]binacle-vipaq[\\/]/,
						name: 'binacle-vipaq',
						chunks: 'all',
						enforce: true,
						priority: 20, // higher than vendors
					},
				},
			},
		},
		cache: {
			type: 'filesystem',
		},
		devtool:  production ? false : 'source-map',
	};
}
