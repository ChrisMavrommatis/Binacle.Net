import {ViPaqSerializer} from "binacle-vipaq";

export default () => ({
	model:{
		result: null
	},
	addResult(){
		console.log(this.model.result);
		const data = Uint8Array.fromBase64(this.model.result);

		ViPaqSerializer.deserialize(data)
			.then(result => {
				this.$logger.error("[Binacle]", result);
			})
			.catch(error => {
				this.$logger.error("[Binacle] Error deserializing ViPaq data:", error);
			});

	}
});
