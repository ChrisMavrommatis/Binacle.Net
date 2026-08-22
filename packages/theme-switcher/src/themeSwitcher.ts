import { Cookies } from 'cookies';

export default class ThemeSwitcherButtonElement extends HTMLElement {
	private _defaultMode = 'light';
	private _themeIcon: HTMLElement | null = null;

	connectedCallback() {
		this._defaultMode = this.dataset.defaultTheme || 'light';
		let themeValue = Cookies.get('theme');
		if(!themeValue){
			// No cookie set, use default mode
			themeValue = this._defaultMode;
		}
		if(themeValue === 'dark') {
			document.body.classList.add('dark');
			document.body.classList.remove('light');
		} else if(themeValue === 'light') {
			document.body.classList.add('light');
			document.body.classList.remove('dark');
		}

		this.render();
		this.changeThemeElementsAccordingToTheme();
		this.addEventListener('click', this.changeTheme.bind(this));
	}

	disconectedCallback() {
		this.removeEventListener('click', this.changeTheme.bind(this));
	}

	changeTheme(){
		if(this.isDarkTheme()){
			document.body.classList.remove("dark");
			document.body.classList.add("light");
			this._themeIcon!.textContent = "dark_mode";
		}else {
			document.body.classList.remove("light");
			document.body.classList.add("dark");
			this._themeIcon!.textContent = "light_mode";
		}
		const themeValue = this.isDarkTheme() ? 'dark' : 'light';

		// Secure only where the browser will keep it. The cookies default is secure, and the API image is
		// commonly served over plain http on a LAN - there the cookie is dropped and the theme resets on
		// every page load.
		Cookies.set('theme', themeValue, {expires: 365, secure: location.protocol === 'https:'});

		this.changeThemeElementsAccordingToTheme();
		const event = new CustomEvent('themeChanged', {detail: {theme: themeValue}});
		window.dispatchEvent(event);
	}

	render() {
		this._themeIcon = document.createElement('i');
		this._themeIcon.classList.add('page', 'top', 'active');
		this._themeIcon.textContent = this.isDarkTheme() ? "light_mode" : "dark_mode"
		this.appendChild(this._themeIcon);
	}

	isDarkTheme(){
		return document.body.classList.contains("dark");
	}

	changeThemeElementsAccordingToTheme() {
		const themeChangingElements = document.querySelectorAll<HTMLElement>('[data-theme]');
		themeChangingElements.forEach(element => {
			const attribute = element.dataset.theme as string;

			const themeValue = this.isDarkTheme()
				? element.dataset.darktheme
				: element.dataset.lighttheme;
			// An element missing data-darktheme or data-lighttheme writes the literal string "undefined".
			// The cast keeps that rather than skipping the element.
			element.setAttribute(attribute, themeValue as string);
		});
	}
}
