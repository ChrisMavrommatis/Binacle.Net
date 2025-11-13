export default class Control {
	constructor(index, id, icon, classes, onClick) {
		this.index = index;
		this.id = id;
		this.icon = icon;
		this.classes = classes;
		this.onClick = onClick;
		this.enabled = false;
	}
}
