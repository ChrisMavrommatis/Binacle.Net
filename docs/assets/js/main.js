/******/ (() => { // webpackBootstrap
/******/ 	"use strict";

;// ./_js/converter.js
class Converter {
    static read(value) {
        if (value[0] === '"') {
            value = value.slice(1, -1)
        }
        return value.replace(/(%[\dA-F]{2})+/gi, decodeURIComponent)
    }

    static write(value) {
        return encodeURIComponent(value).replace(
            /%(2[346BF]|3[AC-F]|40|5[BDE]|60|7[BCD])/g,
            decodeURIComponent
        )
    }
}

/* harmony default export */ const converter = (Converter);
;// ./_js/cookies.js


class Cookies {
    static __defaultAttributes = {
        path: '/',
        expires: 90,
        sameSite: 'Lax',
        secure: true
    };

    static __assign(target) {
        for (let i = 1; i < arguments.length; i++) {
            const source = arguments[i];
            for (const key in source) {
                target[key] = source[key];
            }
        }
        return target;
    }

    static set(key, value, attributes) {
        if (typeof document === 'undefined') {
            return
        }

        attributes = Cookies.__assign({}, Cookies.__defaultAttributes, attributes);

        if (typeof attributes.expires === 'number') {
            attributes.expires = new Date(Date.now() + attributes.expires * 864e5);
        }
        if (attributes.expires) {
            attributes.expires = attributes.expires.toUTCString();
        }

        key = encodeURIComponent(key)
            .replace(/%(2[346B]|5E|60|7C)/g, decodeURIComponent)
            .replace(/[()]/g, escape);

        let stringifiedAttributes = '';
        for (const attributeName in attributes) {
            if (!attributes[attributeName]) {
                continue;
            }

            stringifiedAttributes += '; ' + attributeName;

            if (attributes[attributeName] === true) {
                continue;
            }

            stringifiedAttributes += '=' + attributes[attributeName].split(';')[0];
        }
        return (document.cookie = key + '=' + converter.write(value, key) + stringifiedAttributes);
    }


    static get(key) {
        if (typeof document === 'undefined' || (arguments.length && !key)) {
            return;
        }

        const cookies = document.cookie ? document.cookie.split('; ') : [];
        const jar = {};
        for (let i = 0; i < cookies.length; i++) {
            const parts = cookies[i].split('=');
            const value = parts.slice(1).join('=');

            try {
                const foundKey = decodeURIComponent(parts[0]);
                jar[foundKey] = converter.read(value, foundKey);

                if (key === foundKey) {
                    break;
                }
            } catch (e) {
            }
        }

        return key ? jar[key] : jar;
    }

    static remove(key) {
        Cookies.set(key, '', Cookies.__assign({}, {expires: -1}));
    }
}

/* harmony default export */ const cookies = (Cookies);

;// ./_js/themeSwitcher.js


class ThemeSwitcherButtonElement extends HTMLElement {
    constructor() {
        super();
        this._defaultMode = 'light';
        this._themeIcon = null;
    }



    connectedCallback() {
        this._defaultMode = this.getAttribute("data-default-theme") || 'light';
        let themeValue = cookies.get('theme');
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

        cookies.set('theme', themeValue, {expires: 365});

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

/* harmony default export */ const themeSwitcher = (ThemeSwitcherButtonElement);
;// ./_js/main.js


document.addEventListener('DOMContentLoaded', function () {
    
    const versionSelects = document.querySelectorAll('[data-versionselect]');
    
    if(!!versionSelects){
        versionSelects.forEach(versionSelect => {
            versionSelect.addEventListener('change', function (event) {
                const url = versionSelect.dataset.versionselect;
                const selectedVersion = event.target.value;
                if (selectedVersion) {
                    window.location.href = url + selectedVersion;
                }
            });
        });
    }
    
    // Find the active span
    const activeSpans = document.querySelectorAll('span[data-active]');
    if (!!activeSpans){
        activeSpans.forEach(activeSpan => {
            // Start from the active span and walk up, opening all parent <details>
            let parent = activeSpan.closest('details');
            let count = 0;
            while (parent && count < 10) { // Limit to 10 levels to prevent infinite loop
                parent.open = true;
                parent = parent.parentElement.closest('details');
                count++;
            }
        });    
    }

    customElements.define('theme-switcher', themeSwitcher);

});

/******/ })()
;