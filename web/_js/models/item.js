import {ValidatableBox} from "./validatableBox";

class Item extends ValidatableBox {
	constructor(length, width, height, quantity) {
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

export default Item;
