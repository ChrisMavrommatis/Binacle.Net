import { Bin } from "./bin";
import {PackedItem} from "./packedItem";

export interface SceneData {
	bin: Bin | null;
	items: PackedItem[];
}
