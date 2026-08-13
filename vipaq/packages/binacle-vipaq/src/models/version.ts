// Ports C#: Version. The wire format version (PROTOCOL.md §2.3). Version1 is the only version this
// implementation writes or reads; codes 1-3 are reserved and a decoder rejects them. Version does not carry
// compression - that is its own header bit.
export const enum Version {
	Version1 = 0,
	Reserved1 = 1,
	Reserved2 = 2,
	Reserved3 = 3,
}
