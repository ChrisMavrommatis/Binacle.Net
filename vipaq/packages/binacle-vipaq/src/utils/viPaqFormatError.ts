// Ports C#: ViPaqFormatException. A malformed blob: unsupported version, a set reserved bit, a reserved width
// code, a truncated or over-long body. Its own class so a decode rejection reads differently from a
// caller-argument error. The decode-invalid vectors only assert "throws", so the message is not a contract.
export class ViPaqFormatError extends Error {
	constructor(message: string) {
		super(message);
		this.name = "ViPaqFormatError";
	}
}
