import {Dictionary} from "../models";

export default class ErrorCollection {
	private fields: Dictionary<string[]> = {};

	constructor() {

	}

	push(fieldName: string, message: string) {
		// to lower
		const normalizedFieldName = fieldName.toLowerCase();
		this.fields[normalizedFieldName] = this.fields[normalizedFieldName] || [];
		this.fields[normalizedFieldName].push(message);
	}

	get errorMessages() {
		const messages = [] as string[];
		for (const fieldName in this.fields) {
			for (const errorMessage of this.fields[fieldName]) {
				messages.push(errorMessage);
			}
		}
		return messages;
	}

	hasError(fieldName: string) {
		const normalizedFieldName = fieldName.toLowerCase();
		return (this.fields[normalizedFieldName]?.length ?? 0) > 0;
	}

	hasErrors() {
		for (const fieldName in this.fields) {
			if (this.fields[fieldName].length > 0) {
				return true;
			}
		}
		return false;
	}

}
