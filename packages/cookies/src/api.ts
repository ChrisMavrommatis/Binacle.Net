import assign from './assign'
import converter from './converter'

export interface CookieAttributes {
	path?: string
	domain?: string
	expires?: number | Date | string
	sameSite?: string
	secure?: boolean

	[attribute: string]: string | number | boolean | Date | undefined
}

export default class Cookies {
	static __defaultAttributes: CookieAttributes = {
		path: '/',
		expires: 90,
		sameSite: 'Lax',
		secure: true
	};

	static set(name: string, value: string, attributes?: CookieAttributes): string | undefined {
		if (typeof document === 'undefined') {
			return
		}

		attributes = assign<CookieAttributes>({}, Cookies.__defaultAttributes, attributes)

		if (typeof attributes.expires === 'number') {
			attributes.expires = new Date(Date.now() + attributes.expires * 864e5)
		}
		if (attributes.expires) {
			// A string expires throws here, the same as it did before the port.
			attributes.expires = (attributes.expires as Date).toUTCString()
		}

		name = encodeURIComponent(name)
			.replace(/%(2[346B]|5E|60|7C)/g, decodeURIComponent)
			.replace(/[()]/g, escape)

		let stringifiedAttributes = ''
		for (const attributeName in attributes) {
			const attributeValue = attributes[attributeName]
			if (!attributeValue) {
				continue
			}

			stringifiedAttributes += '; ' + attributeName

			if (attributeValue === true) {
				continue
			}

			// RFC 6265 section 5.2: an attribute value ends at the first ";".
			stringifiedAttributes += '=' + (attributeValue as string).split(';')[0]
		}

		return (document.cookie =
			name + '=' + converter.write(value, name) + stringifiedAttributes)
	}

	static get(): Record<string, string>
	static get(name: string): string | undefined
	static get(name?: string): string | Record<string, string> | undefined {
		// arguments.length, not just !name: a bare get() returns the whole jar, get('') returns nothing.
		if (typeof document === 'undefined' || (arguments.length && !name)) {
			return
		}

		// To prevent the for loop in the first place assign an empty array
		// in case there are no cookies at all.
		const cookies = document.cookie ? document.cookie.split('; ') : []
		const jar: Record<string, string> = {}
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

	static remove(key: string): void {
		Cookies.set(key, '', assign<CookieAttributes>({}, {expires: -1}));
	}
}
