import ErrorCollection from '../models/errorCollection'

export default class ValidatableBox {
	static minAllowedDimension = 1;
	static maxAllowedDimension = 65535;

	constructor(length, width, height) {
		this.length = length;
		this.width = width;
		this.height = height;
	}

	get id() {
		return `${this.length}x${this.width}x${this.height}`;
	}

	isFieldValid(fieldName) {
		const errors = this.errorState;
		if (fieldName) {
			return !errors.hasError(fieldName);
		}
		return true;
	}

	hasErrors() {
		const errors = this.errorState;
		return errors.hasErrors();
	}

	get allErrorMessages() {
		const errors = this.errorState;
		return errors.errorMessages;
	}

	getDimensions() {
		return [
			{name: 'Length', value: Number(this.length)},
			{name: 'Width', value: Number(this.width)},
			{name: 'Height', value: Number(this.height)},
		];
	}

	get errorState() {
		const errors = new ErrorCollection();

		const dimensions = this.getDimensions();

		dimensions.forEach(dimension => {
			if (dimension.value === null || dimension.value === undefined) {
				errors.push(dimension.name, `${dimension.name} is required`);
			}

			if (isNaN(dimension.value)) {
				errors.push(dimension.name,`${dimension.name} must be a number`);
			}

			if (!Number.isInteger(dimension.value)) {
				errors.push(dimension.name,`${dimension.name} must be an integer`);
			}

			if (dimension.value < ValidatableBox.minAllowedDimension || dimension.value > ValidatableBox.maxAllowedDimension) {
				errors.push(
					dimension.name,
					`${dimension.name} must be between ${ValidatableBox.minAllowedDimension} and ${ValidatableBox.maxAllowedDimension}`,
				);
			}
		});

		return errors;
	}
}
