import {getResponseStatusText} from "../../src/utils/getResponseStatusText";

test.each([
	[400, "Bad Request"],
	[401, "Unauthorized"],
	[403, "Forbidden"],
	[404, "Not Found"],
	[422, "Unprocessable Entity"],
	[429, "Too Many Requests"],
	[500, "Internal Server Error"],
	[502, "Bad Gateway"],
	[503, "Service Unavailable"],
])("%i reads as '%s'", (status, expected) => {
	const text = getResponseStatusText(status);

	expect(text).toBe(expected);
});

test.each([0, 200, 418, 504])("%i falls back to 'Error'", (status) => {
	const text = getResponseStatusText(status);

	expect(text).toBe("Error");
});
