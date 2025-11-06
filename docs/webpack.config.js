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
            clean: true,
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
        main
    ];
}