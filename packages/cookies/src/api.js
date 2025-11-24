import assign from './assign'
import converter from './converter'

export default class Cookies {
	static __defaultAttributes = {
		path: '/',
		expires: 90,
		sameSite: 'Lax',
		secure: true
	};

	static set(name, value, attributes) {
		if (typeof document === 'undefined') {
			return
		}

		attributes = assign({}, Cookies.__defaultAttributes, attributes)

		if (typeof attributes.expires === 'number') {
			attributes.expires = new Date(Date.now() + attributes.expires * 864e5)
		}
		if (attributes.expires) {
			attributes.expires = attributes.expires.toUTCString()
		}

		name = encodeURIComponent(name)
			.replace(/%(2[346B]|5E|60|7C)/g, decodeURIComponent)
			.replace(/[()]/g, escape)

		let stringifiedAttributes = ''
		for (const attributeName in attributes) {
			if (!attributes[attributeName]) {
				continue
			}

			stringifiedAttributes += '; ' + attributeName

			if (attributes[attributeName] === true) {
				continue
			}

			// Considers RFC 6265 section 5.2:
			// ...
			// 3.  If the remaining unparsed-attributes contains a %x3B (";")
			//     character:
			// Consume the characters of the unparsed-attributes up to,
			// not including, the first %x3B (";") character.
			// ...
			stringifiedAttributes += '=' + attributes[attributeName].split(';')[0]
		}

		return (document.cookie =
			name + '=' + converter.write(value, name) + stringifiedAttributes)
	}



	static get(name) {
		if (typeof document === 'undefined' || (arguments.length && !name)) {
			return
		}

		// To prevent the for loop in the first place assign an empty array
		// in case there are no cookies at all.
		const cookies = document.cookie ? document.cookie.split('; ') : []
		let jar = {}
		for (let i = 0; i < cookies.length; i++) {
			const parts = cookies[i].split('=')
			const value = parts.slice(1).join('=')

			try {
				const found = decodeURIComponent(parts[0])
				if (!(found in jar)) jar[found] = converter.read(value, found)
				if (name === found) {
					break
				}
			} catch {
				// Do nothing...
			}
		}

		return name ? jar[name] : jar
	}

	static remove(key) {
		Cookies.set(key, '', assign({}, {expires: -1}));
	}
}

