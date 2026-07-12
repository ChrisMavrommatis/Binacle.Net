// ports C#: RoundTripScenarioTests
//
// Encode a (bin, items) input under the scenario's header, pin the two header bytes, then decode and assert the
// bin and items come back unchanged. These drive ProtocolEncoder, not ViPaqSerializer, so the scenario's header
// is an input: that is what lets a scenario be columnar or wider than narrowest. ViPaqSerializer always writes
// raw, row-major, narrowest, so those scenarios are unreachable through it. Every scenario is uncompressed for
// now (compression is deferred, PROTOCOL.md §6).

import ViPaqSerializer from "../src/ViPaqSerializer";
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {headerFromBytes} from "../src/utils";
import {roundTripCases} from "./providers/RoundTripCases";

describe("round-trip scenarios", () => {
	test.each(roundTripCases)("$name", async ({bin, items, expected}) => {
		const data = await new ProtocolEncoder().encode(expected, bin, items);

		// The two header bytes pin version, compression, layout and all three widths at once. A cheap guard that
		// the encoder wrote the header it was handed — the real coverage is the decode below.
		expect(headerFromBytes(data[0], data[1])).toEqual(expected);

		const result = await ViPaqSerializer.deserialize(data);
		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
