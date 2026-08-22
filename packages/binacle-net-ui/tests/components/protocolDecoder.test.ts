import type {Alpine as AlpineType} from "alpinejs";
import {Coordinates, Dimensions, ViPaqSerializer} from "binacle-vipaq";

import {Logger} from "../../src/core/logger";
import {protocolDecoderApp, protocolDecoderAppPlugin} from "../../src/core/protocolDecoder";
import {DecodedPackingResult} from "../../src/viewModels";

const savedResultsKey = "ProtocolDecoderSavedResults";

interface Dispatched {
	name: string;
	detail: any;
}

type Decoder = ReturnType<typeof protocolDecoderApp> & {
	$dispatch: (event: string, detail?: any) => void;
	$logger: Logger;
};

function createDecoder() {
	const dispatched: Dispatched[] = [];
	const decoder = protocolDecoderApp() as Decoder;
	decoder.$dispatch = (name: string, detail?: any) => {dispatched.push({name, detail});};
	decoder.$logger = new Logger(false);

	return {decoder, dispatched};
}

const bin: Dimensions = {length: 10, width: 10, height: 10};
const items: (Dimensions & Coordinates)[] = [
	{length: 2, width: 2, height: 2, x: 0, y: 0, z: 0},
	{length: 3, width: 3, height: 3, x: 2, y: 0, z: 0},
];

function toBase64(bytes: Uint8Array): string {
	return btoa(String.fromCharCode(...bytes));
}

async function encodeToken(): Promise<string> {
	return toBase64(await ViPaqSerializer.serialize(bin, items));
}

// A single byte is shorter than the two header bytes, so deserialize rejects on it every time.
const tooShortToken = toBase64(Uint8Array.from([0]));

// The deserialize calls the component fires are not awaited, so a test has to give the microtask queue a turn.
function settle(): Promise<void> {
	return new Promise(resolve => setTimeout(resolve, 0));
}

function save(payload: unknown): void {
	localStorage.setItem(savedResultsKey, JSON.stringify(payload));
}

function readSaved(): {version: number; results: string[]} {
	return JSON.parse(localStorage.getItem(savedResultsKey)!);
}

beforeEach(() => {
	localStorage.clear();
});

describe("loading saved results", () => {
	test("nothing saved gives an empty list", () => {
		const {decoder} = createDecoder();

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});

	test("nothing saved says nothing to the user", () => {
		const {decoder, dispatched} = createDecoder();

		decoder.loadSavedResults();

		expect(dispatched).toEqual([]);
	});

	test("the current schema version hands the tokens back", () => {
		const {decoder} = createDecoder();
		save({version: 2, results: ["one", "two"]});

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual(["one", "two"]);
	});

	test("an older schema version is discarded", () => {
		const {decoder} = createDecoder();
		save({version: 1, results: ["one"]});

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});

	test("an older schema version tells the user why", () => {
		const {decoder, dispatched} = createDecoder();
		save({version: 1, results: ["one"]});

		decoder.loadSavedResults();

		expect(dispatched).toEqual([{
			name: "error-occurred",
			detail: ["Your saved results were cleared: the packing token format changed and the old saved tokens can no longer be decoded."],
		}]);
	});

	test("an older schema version clears the key", () => {
		const {decoder} = createDecoder();
		save({version: 1, results: ["one"]});

		decoder.loadSavedResults();

		expect(localStorage.getItem(savedResultsKey)).toBeNull();
	});

	test("the old bare array of tokens is discarded", () => {
		const {decoder} = createDecoder();
		save(["one", "two"]);

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});

	test("a body that is not JSON is discarded", () => {
		const {decoder} = createDecoder();
		localStorage.setItem(savedResultsKey, "not json at all");

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});

	test("a body that is not JSON clears the key", () => {
		const {decoder} = createDecoder();
		localStorage.setItem(savedResultsKey, "not json at all");

		decoder.loadSavedResults();

		expect(localStorage.getItem(savedResultsKey)).toBeNull();
	});

	test("a saved body of null is discarded", () => {
		const {decoder} = createDecoder();
		localStorage.setItem(savedResultsKey, "null");

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});

	test("the current version with a non-array results field is discarded", () => {
		const {decoder} = createDecoder();
		save({version: 2, results: "one"});

		const saved = decoder.loadSavedResults();

		expect(saved).toEqual([]);
	});
});

describe("saving results", () => {
	test("writes the tokens under the current schema version", () => {
		const {decoder} = createDecoder();
		decoder.results = [new DecodedPackingResult("token-one", bin, items)];

		decoder.saveResults();

		expect(readSaved()).toEqual({version: 2, results: ["token-one"]});
	});

	test("an empty result set still writes a versioned payload", () => {
		const {decoder} = createDecoder();

		decoder.saveResults();

		expect(readSaved()).toEqual({version: 2, results: []});
	});
});

describe("adding a result", () => {
	test("no token tells the user there is nothing to decode", () => {
		const {decoder, dispatched} = createDecoder();

		decoder.addResult();

		expect(dispatched).toEqual([{name: "error-occurred", detail: ["No ViPaq data to deserialize"]}]);
	});

	test("a token already in the list is refused", async () => {
		const {decoder, dispatched} = createDecoder();
		const token = await encodeToken();
		decoder.results = [new DecodedPackingResult(token, bin, items)];
		decoder.model.result = token;

		decoder.addResult();

		expect(dispatched).toEqual([{name: "error-occurred", detail: ["This ViPaq data has already been added"]}]);
	});

	test("a token already in the list clears the input", async () => {
		const {decoder} = createDecoder();
		const token = await encodeToken();
		decoder.results = [new DecodedPackingResult(token, bin, items)];
		decoder.model.result = token;

		decoder.addResult();

		expect(decoder.model.result).toBeNull();
	});

	test("a token that is not base64 surfaces an error", async () => {
		const {decoder, dispatched} = createDecoder();
		decoder.model.result = "!!! not base64 !!!";

		decoder.addResult();
		await settle();

		expect(dispatched[0].detail[0]).toBe("Error deserializing ViPaq data");
	});

	test("a token that is not base64 adds nothing", async () => {
		const {decoder} = createDecoder();
		decoder.model.result = "!!! not base64 !!!";

		decoder.addResult();
		await settle();

		expect(decoder.results).toEqual([]);
	});

	test("base64 that is not a ViPaq blob surfaces an error", async () => {
		const {decoder, dispatched} = createDecoder();
		decoder.model.result = tooShortToken;

		decoder.addResult();
		await settle();

		expect(dispatched[0].detail[0]).toBe("Error deserializing ViPaq data");
	});

	test("a valid token is decoded into the result list", async () => {
		const {decoder} = createDecoder();
		decoder.model.result = await encodeToken();

		decoder.addResult();
		await settle();

		expect(decoder.results.map(r => r.bin)).toEqual([bin]);
	});

	test("the first valid token becomes the selected result", async () => {
		const {decoder} = createDecoder();
		decoder.model.result = await encodeToken();

		decoder.addResult();
		await settle();

		expect(decoder.selectedResult).toBe(decoder.results[0]);
	});

	test("a valid token clears the input", async () => {
		const {decoder} = createDecoder();
		decoder.model.result = await encodeToken();

		decoder.addResult();
		await settle();

		expect(decoder.model.result).toBeNull();
	});

	test("a valid token is written to localStorage", async () => {
		const {decoder} = createDecoder();
		const token = await encodeToken();
		decoder.model.result = token;

		decoder.addResult();
		await settle();

		expect(readSaved()).toEqual({version: 2, results: [token]});
	});
});

describe("a token round-tripping through localStorage", () => {
	test("comes back as a decoded bin", async () => {
		const adder = createDecoder();
		adder.decoder.model.result = await encodeToken();
		adder.decoder.addResult();
		await settle();
		const {decoder} = createDecoder();

		decoder.init();
		await settle();

		expect(decoder.results.map(r => r.bin)).toEqual([bin]);
	});

	test("comes back as decoded items", async () => {
		const adder = createDecoder();
		adder.decoder.model.result = await encodeToken();
		adder.decoder.addResult();
		await settle();
		const {decoder} = createDecoder();

		decoder.init();
		await settle();

		expect(decoder.results[0].items).toEqual(items);
	});

	test("comes back selected", async () => {
		const adder = createDecoder();
		adder.decoder.model.result = await encodeToken();
		adder.decoder.addResult();
		await settle();
		const {decoder} = createDecoder();

		decoder.init();
		await settle();

		expect(decoder.selectedResult).toBe(decoder.results[0]);
	});

	test("a saved token that no longer decodes surfaces an error", async () => {
		const {decoder, dispatched} = createDecoder();
		save({version: 2, results: [tooShortToken]});

		decoder.init();
		await settle();

		expect(dispatched[0].detail[0]).toBe("Error deserializing saved ViPaq data");
	});
});

describe("deleting a result", () => {
	test("takes it out of the list", () => {
		const {decoder} = createDecoder();
		const first = new DecodedPackingResult("one", bin, items);
		const second = new DecodedPackingResult("two", bin, items);
		decoder.results = [first, second];

		decoder.deleteResult(first);

		expect(decoder.results).toEqual([second]);
	});

	test("rewrites localStorage without it", () => {
		const {decoder} = createDecoder();
		const first = new DecodedPackingResult("one", bin, items);
		const second = new DecodedPackingResult("two", bin, items);
		decoder.results = [first, second];

		decoder.deleteResult(first);

		expect(readSaved()).toEqual({version: 2, results: ["two"]});
	});

	test("deleting the selected one selects what is left", () => {
		const {decoder} = createDecoder();
		const first = new DecodedPackingResult("one", bin, items);
		const second = new DecodedPackingResult("two", bin, items);
		decoder.results = [first, second];
		decoder.selectResult(first);

		decoder.deleteResult(first);

		expect(decoder.selectedResult).toBe(second);
	});

	test("deleting the last one leaves nothing selected", () => {
		const {decoder} = createDecoder();
		const only = new DecodedPackingResult("one", bin, items);
		decoder.results = [only];
		decoder.selectResult(only);

		decoder.deleteResult(only);

		expect(decoder.selectedResult).toBeNull();
	});

	test("deleting one that is not in the list changes nothing", () => {
		const {decoder} = createDecoder();
		const kept = new DecodedPackingResult("one", bin, items);
		decoder.results = [kept];

		decoder.deleteResult(new DecodedPackingResult("other", bin, items));

		expect(decoder.results).toEqual([kept]);
	});
});

describe("selecting a result", () => {
	test("marks it as the selected one", () => {
		const {decoder} = createDecoder();
		const result = new DecodedPackingResult("one", bin, items);

		decoder.selectResult(result);

		expect(decoder.isSelected(result)).toBe(true);
	});

	test("hands the scene a thunk resolving to the result", async () => {
		const {decoder, dispatched} = createDecoder();
		const result = new DecodedPackingResult("one", bin, items);

		decoder.selectResult(result);

		expect(dispatched[0].name).toBe("update-scene");
		await expect(dispatched[0].detail()).resolves.toBe(result);
	});

	test("a result that is not selected reports false", () => {
		const {decoder} = createDecoder();
		const selected = new DecodedPackingResult("one", bin, items);
		const other = new DecodedPackingResult("two", bin, items);
		decoder.selectResult(selected);

		const isSelected = decoder.isSelected(other);

		expect(isSelected).toBe(false);
	});
});

describe("result labels", () => {
	test("the title is the bin's dimensions", () => {
		const {decoder} = createDecoder();
		const result = new DecodedPackingResult("one", bin, items);

		const title = decoder.resultTitle(result);

		expect(title).toBe("Bin: 10x10x10");
	});

	test("the bin percentage is the packed share, rounded", () => {
		const {decoder} = createDecoder();
		const result = new DecodedPackingResult("one", bin, items);

		const text = decoder.resultBinPercentageText(result);

		expect(text).toBe("Packed Bin Volume: 4%");
	});
});

describe("the plugin", () => {
	test("registers the factory under its x-data name", () => {
		const registered: Record<string, unknown> = {};
		const alpine = {data: (name: string, factory: unknown) => {registered[name] = factory;}} as unknown as AlpineType;

		protocolDecoderAppPlugin(alpine);

		expect(registered).toEqual({protocol_decoder_app: protocolDecoderApp});
	});
});
