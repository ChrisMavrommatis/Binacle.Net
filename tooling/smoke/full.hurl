# full - everything on plus DEBUG_ENDPOINT. The dev/demo shape the shipped example stacks turn on: the web UI
# serves and /_debug is deliberately 200. Redirects are off.

GET http://localhost:8080/api/v3/presets
HTTP 200

GET http://localhost:8080/swagger/index.html
HTTP 200

GET http://localhost:8080/scalar/
HTTP 200

GET http://localhost:8080/openapi/v3.json
HTTP 200

GET http://localhost:8080/openapi/v4.json
HTTP 200

GET http://localhost:8080/openapi/service.json
HTTP 200

# Every page route the module serves. The demo is switched on in this profile, so a 404 here means a page
# was renamed on one side only.
GET http://localhost:8080/
HTTP 200

GET http://localhost:8080/packing
HTTP 200

GET http://localhost:8080/vipaq
HTTP 200

GET http://localhost:8080/instance
HTTP 200

# A page with no javascript and no stylesheet still returns 200, so assert the build output itself.
# runtime.js and main.js load on every page, including the ones above that pull in nothing else.
GET http://localhost:8080/_content/Binacle.Net.UIModule/js/runtime.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/js/main.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/js/instance.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/js/packing_demo.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/js/protocol_decoder.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/js/three.js
HTTP 200

GET http://localhost:8080/_content/Binacle.Net.UIModule/css/main.css
HTTP 200

# An unreserved path gets the error page.
GET http://localhost:8080/nope
HTTP 404
[Asserts]
header "Content-Type" contains "text/html"

# A reserved one never does. This is the assertion that catches a module forgetting to declare its paths.
GET http://localhost:8080/api/v3/nope
HTTP 404
[Asserts]
bytes count == 0

# The Web SDK ships the blazor runtime with every project and a target in Binacle.Net.csproj strips it.
# Nothing breaks when that stops working - the image just grows by 552 KB - so assert it.
GET http://localhost:8080/_framework/blazor.web.js
HTTP 404

# health on: Features lists every module. Cross-check against route presence - if a module is listed here but
# its route is 404, AddX ran and UseX did not.
GET http://localhost:8080/_health
HTTP 200
[Asserts]
jsonpath "$.status" == "Healthy"
jsonpath "$.entries.System.data.Version" != "Unknown"
jsonpath "$.entries.System.data.Features" contains "SwaggerUI"
jsonpath "$.entries.System.data.Features" contains "ScalarUI"
jsonpath "$.entries.System.data.Features" contains "UIModule"
jsonpath "$.entries.System.data.Features" contains "ServiceModule"
jsonpath "$.entries.System.data.Features" contains "DebugEndpoint"
# Health checks are on in this profile, and the entry is bound from the config file rather than a flag.
jsonpath "$.entries.System.data.Features" contains "HealthChecks"

# debug endpoint on - echoes the request
GET http://localhost:8080/_debug
HTTP 200

POST http://localhost:8080/api/auth/token
Content-Type: application/json
{
  "Username": "admin@binacle.net",
  "Password": "B1n4cl3Adm!n"
}
HTTP 200
[Asserts]
jsonpath "$.accessToken" exists
jsonpath "$.tokenType" == "Bearer"

GET http://localhost:8080/api/admin/account/11111111-1111-1111-1111-111111111111
HTTP 401
