export default (default_title) => ({
	errors: [],
	defaultTitle: default_title,
	title: default_title,
	hasErrors() {
		return this.errors.length > 0;
	},
	closeDialog(){
		this.errors = [];
		this.title = this.defaultTitle;
	},
	onErrorOccurred(data){
		if(data && data.title){
			this.title = data.title;
		}
		if(data && data.errors){
			this.errors = data.errors;
		}
		//if data is array
		if(data && Array.isArray(data)){
			this.errors = data;
		}
	}
})
