export function stopLoading(container: HTMLElement) {
	const loader = container.querySelector("#loader");
	if (loader) {
		loader.remove();
	}
}
