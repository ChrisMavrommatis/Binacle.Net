export default class Control {
	public index: number;
	public id: string;
	public icon: string;
	public classes: string;
	public onClick: () => void;
	public enabled: boolean;

	constructor(index: number, id: string, icon: string, classes: string, onClick: () => void) {
		this.index = index;
		this.id = id;
		this.icon = icon;
		this.classes = classes;
		this.onClick = onClick;
		this.enabled = false;
	}

	enable() {
		this.enabled = true;
	}

	disable() {
		this.enabled = false;
	}
}
