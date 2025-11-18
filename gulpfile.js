const {src, dest, task} = require('gulp');

const BASE_SRC = 'assets';
const DESTINATIONS = [
	"web",
	"docs"
]

const COPY = {
	images: {
		src: `${BASE_SRC}/**/*.{svg,png,jpg,gif,ico}`,
		dest: DESTINATIONS
	},
	js: {
		src: `${BASE_SRC}/**/*.js`,
		dest: DESTINATIONS
	},
	css: {
		src: `${BASE_SRC}/**/*.css`,
		dest: DESTINATIONS
	},
	fonts: {
		src: `${BASE_SRC}/**/*.woff2`,
		dest: DESTINATIONS
	}
};

task("copy-files", async function () {
	let tasks = [];


	COPY.images.dest.forEach(entry => {
		tasks.push(
			src(COPY.images.src, {encoding:false})
				.pipe(dest(entry))
		)
	});
	COPY.js.dest.forEach(entry => {
		tasks.push(
			src(COPY.js.src)
				.pipe(dest(entry))
		)
	});
	COPY.css.dest.forEach(entry => {
		tasks.push(
			src(COPY.css.src)
				.pipe(dest(entry))
		)
	});
	COPY.fonts.dest.forEach(entry => {
		tasks.push(
			src(COPY.fonts.src)
				.pipe(dest(entry))
		)
	});

	return Promise.all(tasks);
});
