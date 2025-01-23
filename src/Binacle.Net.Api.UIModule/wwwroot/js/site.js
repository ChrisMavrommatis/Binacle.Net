(function(global){

	const modeButton = document.querySelector('[data-role="theme-button"]');
	const themeIcon = modeButton.querySelector('i')
	const body = document.querySelector('body');

	
	function init(){
		themeIcon.textContent =  isDarkTheme() ? "light_mode" : "dark_mode"
		setElementsAccordingToTheme();
		saveThemeCookie();
	}
	
	function saveThemeCookie() {
		Cookies.set('theme', isDarkTheme() ? 'dark' : 'light', { expires: 365 });
	}
	
	function setElementsAccordingToTheme(){
		const themeChangingElements = document.querySelectorAll('[data-theme]');
		themeChangingElements.forEach(element => {
			const attribute = element.getAttribute('data-theme');
			var themeValue = isDarkTheme() ? element.getAttribute("data-darktheme") : element.getAttribute("data-lighttheme");
			element.setAttribute(attribute, themeValue);
		});
	}
	
	function isDarkTheme(){
		return body.classList.contains("dark");
	}
	
	function changeTheme(){
		if(isDarkTheme()) {
			body.classList.remove("dark");
			themeIcon.textContent = "dark_mode";
		} else {
			body.classList.add("dark");
			themeIcon.textContent = "light_mode";
		}
		setElementsAccordingToTheme();
		saveThemeCookie();
	}

	init();
	 modeButton.addEventListener('click', changeTheme);

})(window);