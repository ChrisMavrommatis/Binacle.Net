import Cookies from './cookies';

export default class ThemeSwitcherButtonElement extends HTMLElement {
    constructor() {
        super();
        this._defaultMode = 'light';
        this._themeIcon = null;
    }

    connectedCallback() {
        this._defaultMode = this.getAttribute("data-default-theme") || 'light';
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
            this._themeIcon.textContent = "dark_mode";
        }else {
            document.body.classList.remove("light");
            document.body.classList.add("dark");
            this._themeIcon.textContent = "light_mode";
        }
        const themeValue = this.isDarkTheme() ? 'dark' : 'light';

        Cookies.set('theme', themeValue, {expires: 365});

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
        const themeChangingElements = document.querySelectorAll('[data-theme]');
        themeChangingElements.forEach(element => {
            const attribute = element.getAttribute('data-theme');

            const themeValue = this.isDarkTheme()
                ? element.getAttribute("data-darktheme")
                : element.getAttribute("data-lighttheme");
            element.setAttribute(attribute, themeValue);
        });
    }
}
