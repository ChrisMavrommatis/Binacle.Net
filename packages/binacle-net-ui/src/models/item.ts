import {ValidatableBox} from "./validatableBox";

export class Item extends ValidatableBox {
	public quantity: number;
	constructor(length: number, width: number, height: number, quantity: number) {
		super(length, width, height);
		this.quantity = quantity;
	}

	get id() {
		return `${super.id}-${this.quantity}`;
	}

	getDimensions() {
		const dimensions = super.getDimensions();
		dimensions.push({name: 'Quantity', value: Number(this.quantity)});
		return dimensions;
	}

}

