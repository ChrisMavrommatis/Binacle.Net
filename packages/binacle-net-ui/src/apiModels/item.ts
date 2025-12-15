import {Dimensions} from "../models";

export interface Item extends Dimensions {
	id: string;
	quantity: number;
}

