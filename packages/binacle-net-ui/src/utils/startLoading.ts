export function startLoading(container: HTMLElement) {
	const loader = document.createElement("progress");
	loader.id = "loader";
	loader.classList.add("circle", "large", "absolute", "center", "middle");
	container.append(loader);
}
