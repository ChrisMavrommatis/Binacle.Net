export default function (Alpine) {
	Alpine.magic('logger', function(el, {Alpine}) {
		return {
			enabled: true,
			_logger: window.console,
			log(){
				if (this.enabled) {
					this._logger.log.apply(this._logger, arguments);
				}
			}
		}

	});
}
