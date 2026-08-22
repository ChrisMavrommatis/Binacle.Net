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
// What each target does not get. Everything else in assets/ goes everywhere, so only the weight differs.
//
// swagger-ui is 4.8 MB and only sites/docs/_layouts/versions/swagger.html reads it. The API serves its own
// Swagger UI from Swashbuckle, so it is dead weight on the demo site and in the image.
//
// material-dynamic-colors stays everywhere at 72 KB: sites/demo/_data/includes.yml keeps a commented-out
// script tag for it, so dropping it breaks that line the moment anyone uncomments it.
const IGNORE = {
	docs: [],
	demo: ['assets/lib/swagger-ui/**'],
	uimodule: ['assets/lib/swagger-ui/**']
};

// ----------------- Begin Functions  ----------------- //
// Main Functions
function copyAssets(destinationDir, ignore = []) {
	let tasks = [];
	console.log(`Assets -> ${destinationDir}`);
	Object.keys(ASSETS).forEach(key => {
		const section = {...ASSETS[key], options: {...(ASSETS[key].options || {}), ignore}};
		console.log(`Assets/${key} -> ${destinationDir}`);
		tasks.push(new Promise((resolve) => {
			src(section.src, section.options)
				.pipe(dest(destinationDir))
				.on('end', () =>{
					console.log(`Assets/${key} -> ${destinationDir}: OK`);
					resolve();
				});
		}));
	});
	return Promise.all(tasks).then(() => {
		console.log(`Assets -> ${destinationDir}: OK`);
	});
}

// ----------------- End Functions  ----------------- //

// Tasks
task('copy-assets-to-demo', async function(){
	return copyAssets('sites/demo', IGNORE.demo);
});

task('copy-assets-to-docs', async function(){
	return copyAssets('sites/docs', IGNORE.docs);
});

task('copy-assets-to-uimodule', async function(){
	return copyAssets('api/src/Binacle.Net.UIModule/wwwroot', IGNORE.uimodule);
});
