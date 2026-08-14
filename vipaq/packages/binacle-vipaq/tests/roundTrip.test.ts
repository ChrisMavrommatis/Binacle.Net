// ports C#: RoundTripScenarioTests
//
// Encode under the scenario's header, pin the two header bytes, then decode and assert the bin and items come
// back unchanged. These drive ProtocolEncoder, not ViPaqSerializer, so the header is an input and a scenario
// can be columnar or wider than narrowest. Every scenario is uncompressed.

import ViPaqSerializer from "../src/ViPaqSerializer";
import {ProtocolEncoder} from "../src/ProtocolEncoder";
import {noOpCodec} from "../src/compression";
import {headerFromBytes} from "../src/utils";
import {roundTripCases} from "./providers/RoundTripCases";

describe("round-trip scenarios", () => {
	test.each(roundTripCases)("$name", async ({bin, items, expected}) => {
		const data = await new ProtocolEncoder(noOpCodec).encode(expected, bin, items);

		// A cheap guard that the encoder wrote the header it was handed. The real coverage is the decode below.
		expect(headerFromBytes(data[0], data[1])).toEqual(expected);

		const result = await ViPaqSerializer.deserialize(data);
		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
