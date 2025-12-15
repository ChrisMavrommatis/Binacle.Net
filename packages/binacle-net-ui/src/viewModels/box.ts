import {Dimensions} from "../models";
import ErrorCollection from "./errorCollection";


export default class Box implements Dimensions {
	static minAllowedDimension = 1;
	static maxAllowedDimension = 65535;

	public length: number;
	public width: number
	public height: number;

	constructor(length: number, width: number, height: number) {
		this.length = length;
		this.width = width;
		this.height = height;
	}

	public get id(): string {
		return `${this.length}x${this.width}x${this.height}`;
	}

	isFieldValid(fieldName: string): boolean {
		const errors = this.errorState;
		if (fieldName) {
			return !errors.hasError(fieldName);
		}
		return true;
	}

	hasErrors(): boolean {
		const errors = this.errorState;
		return errors.hasErrors();
	}

	get allErrorMessages(): string[] {
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

	get errorState(): ErrorCollection {
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

			if (dimension.value < Box.minAllowedDimension || dimension.value > Box.maxAllowedDimension) {
				errors.push(
					dimension.name,
					`${dimension.name} must be between ${Box.minAllowedDimension} and ${Box.maxAllowedDimension}`,
				);
			}
		});

		return errors;
	}
}
