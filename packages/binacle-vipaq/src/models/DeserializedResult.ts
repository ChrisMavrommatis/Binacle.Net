import Item from "./Item";
import Bin from "./Bin";

export default class DeserializedResult{
	public bin: Bin;
	public items: Item[];

	constructor(bin: Bin, items: Item[]) {
		this.bin = bin;
		this.items = items;
	}
}
