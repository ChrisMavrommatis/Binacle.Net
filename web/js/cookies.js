/*! based on js-cookie v3.0.5 */
(function (global) {
    /// Cookies
    var defaultAttributes = {
        path: '/',
        expires: 90,
        sameSite: 'Lax',
        secure: true
    };

    var converter = {
        read: function (value) {
            if (value[0] === '"') {
                value = value.slice(1, -1)
            }
            return value.replace(/(%[\dA-F]{2})+/gi, decodeURIComponent)
        },
        write: function (value) {
            return encodeURIComponent(value).replace(
                /%(2[346BF]|3[AC-F]|40|5[BDE]|60|7[BCD])/g,
                decodeURIComponent
            )
        }
    };

    function assignInternal(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                target[key] = source[key];
            }
        }
        return target;
    }

    function setInternal(key, value, attributes) {
        if (typeof document === 'undefined') {
            return
        }

        attributes = assignInternal({}, defaultAttributes, attributes);

        if (typeof attributes.expires === 'number') {
            attributes.expires = new Date(Date.now() + attributes.expires * 864e5);
        }
        if (attributes.expires) {
            attributes.expires = attributes.expires.toUTCString();
        }

        key = encodeURIComponent(key)
            .replace(/%(2[346B]|5E|60|7C)/g, decodeURIComponent)
            .replace(/[()]/g, escape);

        var stringifiedAttributes = '';
        for (var attributeName in attributes) {
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

    function getInternal(key) {
        if (typeof document === 'undefined' || (arguments.length && !key)) {
            return;
        }

        var cookies = document.cookie ? document.cookie.split('; ') : [];
        var jar = {};
        for (var i = 0; i < cookies.length; i++) {
            var parts = cookies[i].split('=');
            var value = parts.slice(1).join('=');

            try {
                var foundKey = decodeURIComponent(parts[0]);
                jar[foundKey] = converter.read(value, foundKey);

                if (key === foundKey) {
                    break;
                }
            } catch (e) {}
        }

        return key ? jar[key] : jar;
    }

    global.Cookies = {
        set: setInternal,
        get: getInternal,
        remove: function (key){
            setInternal(key, '', assignInternal({}, { expires: -1 }));
        },
        defaultAttributes: Object.freeze(defaultAttributes),
        converter: Object.freeze(converter)
    };
})(window);