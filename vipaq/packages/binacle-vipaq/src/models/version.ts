// Ports C#: Version. The wire format version (PROTOCOL.md §2.3). Version1 is the only version this
// implementation writes or reads; codes 1-3 are reserved and a decoder rejects them. Unlike the old enum,
// version no longer carries compression — that is its own bit in the header now (see Header.compressed).
export const enum Version {
	Version1 = 0,
	Reserved1 = 1,
	Reserved2 = 2,
	Reserved3 = 3,
}
