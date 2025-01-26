(function (global) {
	const modeButton = document.querySelector('[data-role="theme-button"]');
	const themeIcon = modeButton.querySelector('i')
	const body = document.querySelector('body');

	function init(){
		themeIcon.textContent =  isDarkTheme() ? "light_mode" : "dark_mode"
		changeThemeElementsAccordingToTheme();
		saveThemeCookie();
	}

	function saveThemeCookie() {
		Cookies.set('theme', isDarkTheme() ? 'dark' : 'light', { expires: 365 });
	}
	function isDarkTheme() {
		return body.classList.contains("dark");
	}
	
	function emitChangeThemeEvent() {
		const theme = isDarkTheme() ? 'dark' : 'light';
		const event = new CustomEvent('themeChanged', { detail: { theme:  theme } });
		window.dispatchEvent(event);
	}
	
	function changeThemeElementsAccordingToTheme(){
		const themeChangingElements = document.querySelectorAll('[data-theme]');
		themeChangingElements.forEach(element => {
			const attribute = element.getAttribute('data-theme');
			var themeValue = isDarkTheme() ? element.getAttribute("data-darktheme") : element.getAttribute("data-lighttheme");
			element.setAttribute(attribute, themeValue);
		});
	}
	
	function changeTheme() {
		if (isDarkTheme()) {
			body.classList.remove("dark");
			body.classList.add("light");
			themeIcon.textContent = "dark_mode";
		} else {
			body.classList.remove("light");
			body.classList.add("dark");
			themeIcon.textContent = "light_mode";
		}

		saveThemeCookie();
		changeThemeElementsAccordingToTheme();
		emitChangeThemeEvent();
	}

	init();
	modeButton.addEventListener('click', changeTheme);
})(window);