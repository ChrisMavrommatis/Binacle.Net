import {deserialize} from './ViPaqSerializer.deserialize'
import {serialize} from './ViPaqSerializer.serialize'

export default class ViPaqSerializer {
	static deserialize = deserialize;
	static serialize = serialize;
}
