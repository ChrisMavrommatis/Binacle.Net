// `_name` is upstream js-cookie's converter shape. This fork has no withConverter, so nothing reads it.
const converter = {
	read: function (value: string, _name?: string): string {
		if (value[0] === '"') {
			value = value.slice(1, -1)
		}
		return value.replace(/(%[\dA-F]{2})+/gi, decodeURIComponent)
	},
	write: function (value: string, _name?: string): string {
		return encodeURIComponent(value).replace(
			/%(2[346BF]|3[AC-F]|40|5[BDE]|60|7[BCD])/g,
			decodeURIComponent
		)
	}
}

export default converter
