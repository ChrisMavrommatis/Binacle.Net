export default class ErrorCollection {

	constructor() {

	}

	push(fieldName, message) {
		// to lower
		const normalizedFieldName = fieldName.toLowerCase();
		this[normalizedFieldName] = this[normalizedFieldName] || [];
		this[normalizedFieldName].push(message);
	}

	get errorMessages() {
		const messages = [];
		for (const fieldName in this) {
			if (this.hasOwnProperty(fieldName)) {
				messages.push(...this[fieldName]);
			}
		}
		return messages;
	}

	hasError(fieldName) {
		const normalizedFieldName = fieldName.toLowerCase();
		return this[normalizedFieldName] && this[normalizedFieldName].length > 0;
	}

	hasErrors() {
		for (const fieldName in this) {
			if (this.hasOwnProperty(fieldName) && this[fieldName].length > 0) {
				return true;
			}
		}
		return false;
	}

}