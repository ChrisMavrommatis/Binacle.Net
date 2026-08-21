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
// What each target does not load. Everything else in assets/ is copied to every target, so the destination
// layout never differs - only the weight does. Measured 21 Aug 2026 by grepping each site for `lib/<name>`
// outside its own lib/ folder; re-measure before changing a line.
//
//   swagger-ui  4.8 MB, and only sites/docs/_layouts/versions/swagger.html reads it. The API serves its own
//               Swagger UI from the Swashbuckle package, so it is dead weight on web and in the image.
//
// material-dynamic-colors stays everywhere: 72 KB, and sites/web/_data/includes.yml keeps a commented-out
// script tag for it, so dropping it would break the line the moment someone uncomments it.
const IGNORE = {
	docs: [],
	web: ['assets/lib/swagger-ui/**'],
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
		tasks.push(new Promise((resolve, reject) => {
			if(section.options){
				src(section.src, section.options)
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
	return copyAssets('sites/web', IGNORE.web);
});

task('copy-assets-to-docs', async function(){
	return copyAssets('sites/docs', IGNORE.docs);
});

task('copy-assets-to-uimodule', async function(){
	return copyAssets('api/src/Binacle.Net.UIModule/wwwroot', IGNORE.uimodule);
});
