import {ValidatableBox} from "./validatableBox";

class Bin extends ValidatableBox {
	constructor(length, width, height) {
		super(length, width, height);
	}

	get id() {
		return `${this.length}x${this.width}x${this.height}`;
	}
}

export default Bin;
