const path = require('path');
const dest = 'js';

module.exports = (env, argv) => {
	const buildType = env.build || 'dist'; // 'dist' or 'watch'
	const production = buildType === 'dist';
	console.log(`Environment Build: ${buildType}`);

	return {
		mode: production ? 'production' : 'development',
		entry: {
			main: './_js/main.js'
		},
		output: {
			filename: '[name].js',
			path: path.resolve(__dirname, dest),
			// Watch mode shares this directory with a running jekyll: deleting a file it has already
			// listed makes its next File.stat raise ENOENT and kills the serve.
			clean: production,
		},
		resolve: {
			extensions: ['.ts', '.js', '.json']
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
		plugins: [
		],
		optimization: {
			minimize: production ? true : false,
			splitChunks: {
				chunks: 'all',
				automaticNameDelimiter: '-',
				minSize: 20000,
				cacheGroups: {
					vendors: {
						test: /[\\/]node_modules[\\/]/,
						name: 'vendors',
						chunks: 'all',
						enforce: true,
						priority: 10,
					}
				},
			},
		},
		cache: {
			type: 'filesystem',
		},
		devtool: production ? false : 'source-map',
	};
}
