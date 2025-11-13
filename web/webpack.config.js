const path = require('path');

const dest = 'js';

module.exports = (env, argv) => {
    this.parallelism = 1;
    console.log(`Environment Build: ${env.build}`);

    const main = {
        entry: './_js/main.js',
        mode: 'production',
        output: {
            filename: 'main.js',
            path: path.resolve(__dirname, dest),
            clean: false,
        },
        module: {
            rules: [
            ],
        },
        plugins: [
        ],
        optimization: {
            minimize: true
        }
    }

    const packing_demo_app ={
        mode: 'production',
        entry: {
			packing_visualizer: {
				import: './_js/components/packing_visualizer.js',
			},
			packing_demo: {
				import: './_js/packing_demo.js',
				dependOn: ['packing_visualizer']
			},
		},
        output: {
            filename: '[name].js',
            path: path.resolve(__dirname, dest),
            clean: false,
        },
        module: {
            rules: [
            ],
        },
        plugins: [
        ],
        optimization: {
            minimize: false
        }

    }

    return [
        main,
        packing_demo_app
    ];
}
