import { SceneData } from "./sceneData";
import { Control } from "./control";

export interface ControlsManager {
	first: Control;
	previous: Control;
	repeat: Control;
	next: Control;
	last: Control;

	all(): Control[];

	disableAll(): void;

	enableAll(): void;

	updateStatus(sceneData: SceneData, itemsRendered: number): void;
}
