export default function (Alpine) {
	Alpine.magic('logger', function(el, {Alpine}) {
		return {
			enabled: true,
			_logger: window.console,
			info() {
				if(this.enabled) {
					this._logger.info.apply(this._logger, arguments);
				}
			},
			log(){
				if (this.enabled) {
					this._logger.log.apply(this._logger, arguments);
				}
			},
			warn(){
				if (this.enabled) {
					this._logger.warn.apply(this._logger, arguments);
				}
			},
			error(){
				if (this.enabled) {
					this._logger.error.apply(this._logger, arguments);
				}
			}
		}

	});
}
