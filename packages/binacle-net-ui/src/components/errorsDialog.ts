import Error from "../models/error";
type ErrorData = string[] | Error;

export default (default_title: string) => ({
	errors: [] as string[],
	defaultTitle: default_title,
	title: default_title,
	hasErrors() {
		return this.errors.length > 0;
	},
	closeDialog(){
		this.errors = [];
		this.title = this.defaultTitle;
	},
	onErrorOccurred(data: ErrorData){
		//if data is array
		if(data && Array.isArray(data)){
			this.errors = data;
		}
		if(typeof data === typeof Error){
			const error = data as Error;
			if(error.title){
				this.title = error.title;
			}
			if(error.errors){
				this.errors = error.errors;
			}
		}
	}
})
