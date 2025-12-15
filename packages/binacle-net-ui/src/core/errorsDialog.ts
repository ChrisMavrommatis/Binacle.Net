import type { Alpine as AlpineType } from 'alpinejs';
import {defineComponent} from "../utils";
import {Error} from "../viewModels";

export function errorsDialogPlugin(Alpine: AlpineType) {
	Alpine.data('errors_dialog', errorsDialog);
}

export const errorsDialog = defineComponent((default_title: string) => ({
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
	onErrorOccurred(data: string[] | Error){
		if(data && Array.isArray(data)){
			this.errors = data;
		}
		if(data instanceof Error){
			const error = data as Error;
			if(error.title){
				this.title = error.title;
			}
			if(error.errors){
				this.errors = error.errors;
			}
		}

	}
}));
