import type {Alpine as AlpineType} from "alpinejs";

import {PackedData, PackingResponse} from "../../src/apiModels/packingResponse";
import {Logger} from "../../src/core/logger";
import {packingDemoApp, packingDemoAppPlugin, PackingDemoOptions} from "../../src/core/packingDemo";
import {largestBin} from "../../src/utils/samples";
import {Bin, Item} from "../../src/viewModels";

// The only place the endpoint path is written. Moving the demo to v4 is this one line.
const packEndpoint = "/api/v3/pack/by-custom";

interface Dispatched {
	name: string;
	detail: any;
}

type SceneThunk = () => Promise<{bin: unknown; items: unknown} | null>;

type PackingDemo = ReturnType<typeof packingDemoApp> & {
	$dispatch: (event: string, detail?: any) => void;
	$logger: Logger;
};

function createApp(options: PackingDemoOptions = {}) {
	const dispatched: Dispatched[] = [];
	const logger = {info: jest.fn(), log: jest.fn(), warn: jest.fn(), error: jest.fn()};
	const app = packingDemoApp(options) as PackingDemo;
	app.$dispatch = (name: string, detail?: any) => {dispatched.push({name, detail});};
	app.$logger = logger as unknown as Logger;

	return {app, dispatched, logger};
}

// The response the component reads is only ever consumed through status, statusText and json(), so a stub is
// enough and a real Response would need a body stream per case.
function stubResponse(status: number, statusText: string, body: unknown | (() => Promise<never>)): Response {
	return {
		status,
		statusText,
		json: typeof body === "function" ? body : () => Promise.resolve(body),
	} as unknown as Response;
}

function mockFetch(response: Response) {
	const fetchMock = jest.fn().mockResolvedValue(response);
	globalThis.fetch = fetchMock as unknown as typeof fetch;

	return fetchMock;
}

function mockFailingFetch(reason: unknown) {
	const fetchMock = jest.fn().mockRejectedValue(reason);
	globalThis.fetch = fetchMock as unknown as typeof fetch;

	return fetchMock;
}

function packedData(overrides: Partial<PackedData> = {}): PackedData {
	return {
		result: "FullyPacked",
		bin: {id: "10x10x10", length: 10, width: 10, height: 10},
		packedItems: [],
		unpackedItems: null,
		packedItemsVolumePercentage: 100,
		packedBinVolumePercentage: 42,
		viPaqData: null,
		...overrides,
	};
}

function packingResponse(data: PackedData[] | null): PackingResponse {
	return {result: "Success", data: data as PackedData[]};
}

// The one scene thunk onSubmit hands over, so a test can run the request the way the visualizer would.
function sceneThunk(dispatched: Dispatched[]): SceneThunk {
	return dispatched.find(x => x.name === "update-scene")!.detail as SceneThunk;
}

describe("init", () => {
	test("seeds bins", () => {
		const {app} = createApp();

		app.init();

		expect(app.model.bins.length).toBeGreaterThan(0);
	});

	test("seeds items", () => {
		const {app} = createApp();

		app.init();

		expect(app.model.items.length).toBeGreaterThan(0);
	});

	test("picks the first algorithm", () => {
		const {app} = createApp();

		app.init();

		expect(app.model.algorithm).toBe("FFD");
	});

	test("the seeded model is submittable", () => {
		const {app} = createApp();

		app.init();

		expect(app.isValid()).toBe(true);
	});

	test("the seeded items fit the seeded bin", () => {
		const {app} = createApp();

		app.init();

		const bin = largestBin(app.model.bins);
		expect(app.model.items.every(i => i.length <= bin.length && i.width <= bin.width && i.height <= bin.height))
			.toBe(true);
	});
});

describe("isValid", () => {
	test("a good model is valid", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];

		const valid = app.isValid();

		expect(valid).toBe(true);
	});

	test("a bin with a dimension below the floor is not valid", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(0, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];

		const valid = app.isValid();

		expect(valid).toBe(false);
	});

	test("a bin with a dimension above the ceiling is not valid", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 65536, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];

		const valid = app.isValid();

		expect(valid).toBe(false);
	});

	test("an item with a dimension that is not a number is not valid", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(Number.NaN, 2, 2, 1)];

		const valid = app.isValid();

		expect(valid).toBe(false);
	});

	test("an item with a fractional dimension is not valid", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2.5, 2, 2, 1)];

		const valid = app.isValid();

		expect(valid).toBe(false);
	});

	// Both lists are checked with `every`, which is true on an empty list. See the report: an emptied form
	// passes the guard and posts a request with no bins and no items.
	test("an emptied model is reported as valid", () => {
		const {app} = createApp();
		app.model.bins = [];
		app.model.items = [];

		const valid = app.isValid();

		expect(valid).toBe(true);
	});
});

describe("the submit guard", () => {
	test("an invalid model sends nothing", () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(0, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];

		app.onSubmit();

		expect(dispatched).toEqual([]);
	});

	test("an invalid model is logged", () => {
		const {app, logger} = createApp();
		app.model.bins = [new Bin(0, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];

		app.onSubmit();

		expect(logger.error).toHaveBeenCalledWith("[Binacle] Model is not valid");
	});
});

describe("adding a bin", () => {
	test("copies the last bin rather than rolling a new one", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(60, 60, 60), new Bin(31, 32, 33)];

		app.addBin["@click"].call(app);

		expect(app.model.bins[2]).toEqual(new Bin(31, 32, 33));
	});

	test("the copy is a new instance", () => {
		const {app} = createApp();
		const last = new Bin(31, 32, 33);
		app.model.bins = [last];

		app.addBin["@click"].call(app);

		expect(app.model.bins[1]).not.toBe(last);
	});

	test("with no bins it rolls one inside the sample bounds", () => {
		const {app} = createApp();
		app.model.bins = [];

		app.addBin["@click"].call(app);

		const rolled = app.model.bins[0];
		expect([rolled.length, rolled.width, rolled.height].every(side => side >= 30 && side <= 60)).toBe(true);
	});
});

describe("editing the lists", () => {
	test("removing a bin drops the one at that index", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 10, 10), new Bin(20, 20, 20), new Bin(30, 30, 30)];

		app.removeBin(1);

		expect(app.model.bins.map(b => b.id)).toEqual(["10x10x10", "30x30x30"]);
	});

	test("clearing the bins empties the list", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];

		app.clearAllBins["@click"].call(app);

		expect(app.model.bins).toEqual([]);
	});

	test("removing an item drops the one at that index", () => {
		const {app} = createApp();
		app.model.items = [new Item(1, 1, 1, 1), new Item(2, 2, 2, 1)];

		app.removeItem(0);

		expect(app.model.items.map(i => i.id)).toEqual(["2x2x2-1"]);
	});

	test("clearing the items empties the list", () => {
		const {app} = createApp();
		app.model.items = [new Item(1, 1, 1, 1)];

		app.clearAllItems["@click"].call(app);

		expect(app.model.items).toEqual([]);
	});

	test("a new item is sized to fit the largest bin", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(40, 40, 40), new Bin(60, 60, 60)];

		app.addItem["@click"].call(app);

		const added = app.model.items[0];
		expect([added.length, added.width, added.height].every(side => side <= 30)).toBe(true);
	});

	test("a new item has a quantity of one", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(40, 40, 40)];

		app.addItem["@click"].call(app);

		expect(app.model.items[0].quantity).toBe(1);
	});

	test("the sizing bin is the largest by volume", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(60, 10, 10), new Bin(30, 30, 30), new Bin(10, 60, 10)];

		const bin = app.sizingBin();

		expect(bin.id).toBe("30x30x30");
	});

	test("with no bins the sizing bin is a rolled one", () => {
		const {app} = createApp();
		app.model.bins = [];

		const bin = app.sizingBin();

		expect([bin.length, bin.width, bin.height].every(side => side >= 30 && side <= 60)).toBe(true);
	});
});

describe("randomize", () => {
	test("replaces the bins", () => {
		const {app} = createApp();
		app.init();
		const before = app.model.bins;

		app.randomize["@click"].call(app);

		expect(app.model.bins).not.toBe(before);
	});

	test("replaces the items in the same call", () => {
		const {app} = createApp();
		app.init();
		const before = app.model.items;

		app.randomize["@click"].call(app);

		expect(app.model.items).not.toBe(before);
	});

	// The bug this replaced was two independent rolls, which could leave items no bin could hold.
	test("the new items fit the new largest bin", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(65535, 65535, 65535)];
		app.model.items = [];

		app.randomize["@click"].call(app);

		const bin = largestBin(app.model.bins);
		expect(app.model.items.every(i => i.length <= bin.length && i.width <= bin.width && i.height <= bin.height))
			.toBe(true);
	});

	test("the new items do not outgrow the new largest bin", () => {
		const {app} = createApp();
		app.model.bins = [new Bin(65535, 65535, 65535)];
		app.model.items = [];

		app.randomize["@click"].call(app);

		const bin = largestBin(app.model.bins);
		const itemsVolume = app.model.items.reduce((sum, i) => sum + i.length * i.width * i.height * i.quantity, 0);
		expect(itemsVolume).toBeLessThanOrEqual(bin.length * bin.width * bin.height);
	});

	test("the rolled set is submittable", () => {
		const {app} = createApp();

		app.randomize["@click"].call(app);

		expect(app.isValid()).toBe(true);
	});
});

describe("the request", () => {
	test("goes to the pack endpoint", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(fetchMock.mock.calls[0][0]).toBe(packEndpoint);
	});

	test("a baseUrl is put in front of the endpoint", async () => {
		const {app, dispatched} = createApp({baseUrl: "https://api.example.com"});
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(fetchMock.mock.calls[0][0]).toBe(`https://api.example.com${packEndpoint}`);
	});

	test("is a POST", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(fetchMock.mock.calls[0][1].method).toBe("POST");
	});

	test("declares a JSON body", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(fetchMock.mock.calls[0][1].headers).toEqual({"Content-Type": "application/json"});
	});

	test("carries the chosen algorithm", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.model.algorithm = "BFD";
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(JSON.parse(fetchMock.mock.calls[0][1].body).parameters).toEqual({algorithm: "BFD"});
	});

	test("maps the bin view models to plain api bins", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 20, 30), new Bin(40, 50, 60)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(JSON.parse(fetchMock.mock.calls[0][1].body).bins).toEqual([
			{id: "10x20x30", length: 10, width: 20, height: 30},
			{id: "40x50x60", length: 40, width: 50, height: 60},
		]);
	});

	test("maps the item view models to plain api items, quantity included", async () => {
		const {app, dispatched} = createApp();
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 3, 4, 5)];
		app.onSubmit();
		const fetchMock = mockFetch(stubResponse(200, "OK", packingResponse([])));

		await sceneThunk(dispatched)();

		expect(JSON.parse(fetchMock.mock.calls[0][1].body).items).toEqual([
			{id: "2x3x4-5", length: 2, width: 3, height: 4, quantity: 5},
		]);
	});
});

describe("an error response", () => {
	test("a 422 field-errors bag becomes one line per field error", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(422, "Unprocessable Entity", {
			title: "Validation failed",
			errors: {Bins: ["Bins is required", "Bins must not be empty"], Items: ["Items is required"]},
		});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.errors).toEqual([
			"Bins: Bins is required",
			"Bins: Bins must not be empty",
			"Items: Items is required",
		]);
	});

	test("a 422 takes its title from the body", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(422, "Unprocessable Entity", {
			title: "Validation failed",
			errors: {Bins: ["Bins is required"]},
		});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.title).toBe("Validation failed");
	});

	test("a 422 puts the detail ahead of the field errors", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(422, "Unprocessable Entity", {
			title: "Validation failed",
			detail: "One or more fields are invalid",
			errors: {Bins: ["Bins is required"]},
		});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.errors).toEqual(["One or more fields are invalid", "Bins: Bins is required"]);
	});

	test("a plain problem response shows its detail", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(400, "Bad Request", {title: "Bad Request", detail: "Algorithm is unknown"});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail).toEqual({title: "Bad Request", errors: ["Algorithm is unknown"]});
	});

	test("a field-errors bag on a status other than 422 is ignored", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(400, "Bad Request", {
			title: "Bad Request",
			errors: {Bins: ["Bins is required"]},
		});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.errors).toEqual([]);
	});

	test("a body with neither title nor detail falls back to the status text", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(503, "Service Unavailable", {});

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail).toEqual({title: "Error: Service Unavailable", errors: []});
	});

	test("an empty body falls back to the status text", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(404, "Not Found", null);

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail).toEqual({title: "Error: Not Found", errors: []});
	});

	test("a body that will not parse says so", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(500, "Internal Server Error", () => Promise.reject(new SyntaxError("bad json")));

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.errors).toEqual(["An error occurred, but the error response could not be parsed."]);
	});

	test("a body that will not parse still names the status", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(500, "Internal Server Error", () => Promise.reject(new SyntaxError("bad json")));

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.title).toBe("Error: Internal Server Error");
	});

	test("a missing status text is looked up from the status", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(429, "", () => Promise.reject(new SyntaxError("bad json")));

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.title).toBe("Error: Too Many Requests");
	});

	test("an unknown status with no status text still gets a title", async () => {
		const {app, dispatched} = createApp();
		const response = stubResponse(418, "", () => Promise.reject(new SyntaxError("bad json")));

		await app.handleErrorResponse(response);

		expect(dispatched[0].detail.title).toBe("Error: Error");
	});
});

describe("getResults", () => {
	const request = {parameters: {algorithm: "FFD"}, bins: [], items: []};

	test("a 200 hands back the parsed body", async () => {
		const {app} = createApp();
		const body = packingResponse([packedData()]);
		mockFetch(stubResponse(200, "OK", body));

		const result = await app.getResults(request);

		expect(result).toEqual(body);
	});

	test("a non-200 hands back nothing", async () => {
		const {app} = createApp();
		mockFetch(stubResponse(400, "Bad Request", {title: "Bad Request"}));

		const result = await app.getResults(request);

		expect(result).toBeNull();
	});

	test("a non-200 surfaces the error", async () => {
		const {app, dispatched} = createApp();
		mockFetch(stubResponse(400, "Bad Request", {title: "Bad Request", detail: "Algorithm is unknown"}));

		await app.getResults(request);

		expect(dispatched).toEqual([{
			name: "error-occurred",
			detail: {title: "Bad Request", errors: ["Algorithm is unknown"]},
		}]);
	});

	test("fetch throwing hands back nothing", async () => {
		const {app} = createApp();
		mockFailingFetch(new TypeError("Failed to fetch"));

		const result = await app.getResults(request);

		expect(result).toBeNull();
	});

	test("fetch throwing surfaces the message", async () => {
		const {app, dispatched} = createApp();
		mockFailingFetch(new TypeError("Failed to fetch"));

		await app.getResults(request);

		expect(dispatched).toEqual([{
			name: "error-occurred",
			detail: {title: "Error while fetching packing results", errors: ["Failed to fetch"]},
		}]);
	});

	test("fetch throwing is logged", async () => {
		const {app, logger} = createApp();
		const reason = new TypeError("Failed to fetch");
		mockFailingFetch(reason);

		await app.getResults(request);

		expect(logger.error).toHaveBeenCalledWith("[Binacle] Error while fetching packing results", reason);
	});

	test("a rejection that is not an error is stringified", async () => {
		const {app, dispatched} = createApp();
		mockFailingFetch("the network went away");

		await app.getResults(request);

		expect(dispatched[0].detail.errors).toEqual(["the network went away"]);
	});
});

describe("the scene the results feed", () => {
	function submit(app: PackingDemo) {
		app.model.bins = [new Bin(10, 10, 10)];
		app.model.items = [new Item(2, 2, 2, 1)];
		app.onSubmit();
	}

	test("the first result with a bin is selected", async () => {
		const {app, dispatched} = createApp();
		const packed = packedData({result: "PartiallyPacked"});
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse([packedData({bin: null as any}), packed])));

		await sceneThunk(dispatched)();

		expect(app.selectedResult).toEqual(packed);
	});

	test("every result is kept for the list", async () => {
		const {app, dispatched} = createApp();
		const data = [packedData({result: "FullyPacked"}), packedData({result: "PartiallyPacked"})];
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse(data)));

		await sceneThunk(dispatched)();

		expect(app.results).toEqual(data);
	});

	test("the scene gets the selected bin and its packed items", async () => {
		const {app, dispatched} = createApp();
		const packedItems = [{id: "2x2x2-1", length: 2, width: 2, height: 2, quantity: 1, x: 0, y: 0, z: 0}];
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse([packedData({packedItems})])));

		const scene = await sceneThunk(dispatched)();

		expect(scene).toEqual({bin: {id: "10x10x10", length: 10, width: 10, height: 10}, items: packedItems});
	});

	test("a result with no packed items gives the scene an empty list", async () => {
		const {app, dispatched} = createApp();
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse([packedData({packedItems: null})])));

		const scene = await sceneThunk(dispatched)();

		expect(scene!.items).toEqual([]);
	});

	test("no result with a bin leaves nothing selected", async () => {
		const {app, dispatched} = createApp();
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse([packedData({bin: null as any})])));

		await sceneThunk(dispatched)();

		expect(app.selectedResult).toBeNull();
	});

	test("a body with no data clears the results", async () => {
		const {app, dispatched} = createApp();
		app.results = [packedData()];
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse(null)));

		await sceneThunk(dispatched)();

		expect(app.results).toEqual([]);
	});

	test("a body with no data leaves the scene empty", async () => {
		const {app, dispatched} = createApp();
		submit(app);
		mockFetch(stubResponse(200, "OK", packingResponse(null)));

		const scene = await sceneThunk(dispatched)();

		expect(scene).toBeNull();
	});

	test("a failed request clears the selection", async () => {
		const {app, dispatched} = createApp();
		app.selectedResult = packedData();
		submit(app);
		mockFailingFetch(new TypeError("Failed to fetch"));

		await sceneThunk(dispatched)();

		expect(app.selectedResult).toBeNull();
	});
});

describe("picking a result from the list", () => {
	test("marks it as selected", () => {
		const {app} = createApp();
		const result = packedData();

		app.selectResult(result);

		expect(app.isSelected(result)).toBe(true);
	});

	test("another result is not selected", () => {
		const {app} = createApp();
		const chosen = packedData();
		app.selectResult(chosen);

		const isSelected = app.isSelected(packedData());

		expect(isSelected).toBe(false);
	});

	test("hands the scene that result's bin and items", async () => {
		const {app, dispatched} = createApp();
		const packedItems = [{id: "2x2x2-1", length: 2, width: 2, height: 2, quantity: 1, x: 1, y: 2, z: 3}];
		const result = packedData({packedItems});

		app.selectResult(result);

		const scene = await sceneThunk(dispatched)();
		expect(scene).toEqual({bin: result.bin, items: packedItems});
	});
});

describe("result labels", () => {
	test("a fully packed result is green", () => {
		const {app} = createApp();
		const result = packedData({result: "FullyPacked"});

		const colour = app.colorClass(result);

		expect(colour).toBe("green");
	});

	test("a partially packed result is orange", () => {
		const {app} = createApp();
		const result = packedData({result: "PartiallyPacked"});

		const colour = app.colorClass(result);

		expect(colour).toBe("orange");
	});

	test("anything else is red", () => {
		const {app} = createApp();
		const result = packedData({result: "NotPacked"});

		const colour = app.colorClass(result);

		expect(colour).toBe("red");
	});

	test("the title names the bin", () => {
		const {app} = createApp();
		const result = packedData();

		const title = app.resultTitle(result);

		expect(title).toBe("Bin: 10x10x10");
	});

	test("the bin percentage reads as a percentage", () => {
		const {app} = createApp();
		const result = packedData({packedBinVolumePercentage: 42});

		const text = app.resultBinPercentageText(result);

		expect(text).toBe("Packed Bin Volume: 42%");
	});

	test("the items percentage reads as a percentage", () => {
		const {app} = createApp();
		const result = packedData({packedItemsVolumePercentage: 87});

		const text = app.resultItemPercentageText(result);

		expect(text).toBe("Packed Items Volume: 87%");
	});

	test("only a fully packed result reports as fully packed", () => {
		const {app} = createApp();
		const result = packedData({result: "PartiallyPacked"});

		const fullyPacked = app.resultIsFullyPacked(result);

		expect(fullyPacked).toBe(false);
	});
});

describe("the plugin", () => {
	test("registers the factory under its x-data name", () => {
		const registered: Record<string, unknown> = {};
		const alpine = {data: (name: string, factory: unknown) => {registered[name] = factory;}} as unknown as AlpineType;

		packingDemoAppPlugin(alpine);

		expect(registered).toEqual({packing_demo_app: packingDemoApp});
	});
});
