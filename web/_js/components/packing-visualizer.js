class Control {
	constructor(index, id, icon, classes, onClick) {
		this.index = index;
		this.id = id;
		this.icon = icon;
		this.classes = classes;
		this.onClick = onClick;
		this.enabled = false;
	}
}

export default () => ({
	controls: {	},
	first(){

	},
	previous(){

	},
	repeat(){

	},
	next(){

	},
	last(){

	},
	init(){
		this.controls = {
			first: new Control(0,"control-first", "first_page",'bottom-left-round', this.first),
			previous: new Control(1,"control-previous", "chevron_left", 'zero-round', this.previous),
			repeat: new Control(2,"control-repeat", "repeat_one",'zero-round', this.repeat),
			next: new Control(3,"control-next", "chevron_right", 'zero-round', this.next),
			last: new Control(4,"control-last", "last_page",'bottom-right-round', this.last),
		};
	}

});
