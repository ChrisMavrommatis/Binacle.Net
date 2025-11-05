import Converter from './converter.js';

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
        return (document.cookie = key + '=' + Converter.write(value, key) + stringifiedAttributes);
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
                jar[foundKey] = Converter.read(value, foundKey);

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

export default Cookies
