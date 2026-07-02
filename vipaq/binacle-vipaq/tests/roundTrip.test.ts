// ports C#: RoundTripScenarioTests
//
// Serialize, pin byte 0 to the expected header (Version + all 3 bit sizes), then deserialize and assert
// the bin and items come back unchanged. The header check is what makes this stronger than plain
// round-trip equality — wrong widths or a wrong compression flag would still round-trip.

import ViPaqSerializer from "../src/ViPaqSerializer";
import {encodingInfoFromByte} from "../src/utils";
import {roundTripCases} from "./providers/RoundTripCases";

describe("round-trip scenarios", () => {
	test.each(roundTripCases)("$name", async ({bin, items, expected}) => {
		const data = await ViPaqSerializer.serialize(bin, items);

		// byte 0 pins Version and all three bit sizes at once.
		const header = encodingInfoFromByte(data[0]);
		expect(header.version).toBe(expected.version);
		expect(header.binDimensionsBitSize).toBe(expected.binDimensionsBitSize);
		expect(header.itemDimensionsBitSize).toBe(expected.itemDimensionsBitSize);
		expect(header.itemCoordinatesBitSize).toBe(expected.itemCoordinatesBitSize);

		const result = await ViPaqSerializer.deserialize(data);
		expect(result.bin).toEqual(bin);
		expect(result.items).toEqual(items);
	});
});
