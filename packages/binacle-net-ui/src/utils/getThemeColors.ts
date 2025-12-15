export function getThemeColors(el: HTMLElement, themeColor:string) {
	const style = window.getComputedStyle(el);
	const color = style.getPropertyValue(`--${themeColor}`);
	const onColor = style.getPropertyValue(`--on-${themeColor}`);
	return {
		color: Number(color.replace('#', '0x')),
		onColor: Number(onColor.replace('#', '0x'))
	};
}
