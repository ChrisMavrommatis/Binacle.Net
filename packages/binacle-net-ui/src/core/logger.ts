import {Alpine as AlpineType } from 'alpinejs'

export class Logger{
	private readonly enabled: boolean;
	private readonly logger: any;
	constructor(enabled: boolean, console: Console = globalThis.console) {
		this.enabled = enabled;
		this.logger = console;
	}
	info(...args: any[]){
		if(this.enabled) {
			this.logger.info(...args);
		}
	}
	log(...args: any[]){
		if (this.enabled) {
			this.logger.log(...args);
		}
	}
	warn(...args: any[]){
		if (this.enabled) {
			this.logger.warn(...args);
		}
	}
	error(...args: any[]){
		if (this.enabled) {
			this.logger.error(...args);
		}
	}
}

export function loggerPlugin(Alpine: AlpineType) {
	Alpine.magic('logger', function(el, {Alpine}) {
		return new Logger(true);
	});
}
