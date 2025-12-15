import {Control} from "../viewModels";
import {SceneData} from "../models";

export default class ControlsManager {
	first: Control;
	previous: Control;
	repeat: Control;
	next: Control;
	last: Control;

	constructor(
		first: Control,
		previous: Control,
		repeat: Control,
		next: Control,
		last: Control
	) {
		this.first = first;
		this.previous = previous;
		this.repeat = repeat;
		this.next = next;
		this.last = last;
	}

	all(): Control[] {
		return [
			this.first,
			this.previous,
			this.repeat,
			this.next,
			this.last
		];
	}

	disableAll(): void{
		this.all().forEach(control => control.disable());
	}

	enableAll(): void {
		this.all().forEach(control => control.enable());
	}

	updateStatus(sceneData: SceneData, itemsRendered: number): void{
		if (!sceneData.bin || !sceneData.items || sceneData.items.length <= 0) {
			this.disableAll();
			return;
		}

		this.repeat.enable();

		if (itemsRendered <= 0) {
			this.first.disable();
			this.previous.disable();
			this.next.enable();
			this.last.enable();
			return;
		}

		if (itemsRendered >= sceneData.items.length) {
			this.first.enable();
			this.previous.enable();
			this.next.disable();
			this.last.disable();
			return;
		}

		this.enableAll();
	}
}
