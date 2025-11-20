const {src, dest, task} = require('gulp');


const ASSETS = {
	images: {
		src: `assets/**/*.{svg,png,jpg,gif,ico}`,
		options: {encoding: false}
	},
	js: {
		src: `assets/**/*.js`
	},
	css: {
		src: `assets/**/*.css`
	},
	fonts: {
		src: `assets/**/*.woff2`,
		options: {encoding: false}
	}
};
// ----------------- Begin Functions  ----------------- //
// Main Functions
function copyAssets(destinationDir) {
	let tasks = [];
	console.log(`Assets -> ${destinationDir}`);
	Object.keys(ASSETS).forEach(key => {
		const section = ASSETS[key];
		console.log(`Assets/${key} -> ${destinationDir}`);
		tasks.push(new Promise((resolve, reject) => {
			if(section.options){
				src(section.src, section.options || {})
					.pipe(dest(destinationDir))
					.on('end', () =>{
						console.log(`Assets/${key} -> ${destinationDir}: OK`);
						resolve();
					});
			}
			else {
				src(section.src)
					.pipe(dest(destinationDir))
					.on('end', () =>{
						console.log(`Assets/${key} -> ${destinationDir}: OK`);
						resolve();
					});
			}

		}));
	});
	return Promise.all(tasks).then(() => {
		console.log(`Assets -> ${destinationDir}: OK`);
	});
}

// ----------------- End Functions  ----------------- //

// Tasks
task('copy-assets-to-web', async function(){
	return copyAssets('web');
});

task('copy-assets-to-docs', async function(){
	return copyAssets('docs');
});
