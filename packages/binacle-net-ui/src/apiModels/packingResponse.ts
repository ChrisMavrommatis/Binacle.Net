import {PackedItem} from "./packedItem";
import {Bin} from "./bin";
import {UnpackedItem} from "./unpackedItem";

export interface PackingResponse {
	result: string;
	data: PackedData[]
}

export interface PackedData {
	result: string;
	bin: Bin,
	packedItems: PackedItem[] | null;
	unpackedItems: UnpackedItem[] | null;
	packedItemsVolumePercentage: number;
	packedBinVolumePercentage: number;
	viPaqData: string | null;

}
