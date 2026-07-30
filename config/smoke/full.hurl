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

GET http://localhost:8080/
HTTP 200

# health on: Features lists every module. Cross-check against route presence - if a module is listed here but
# its route is 404, AddX ran and UseX did not.
GET http://localhost:8080/_health
HTTP 200
[Asserts]
jsonpath "$.status" == "Healthy"
jsonpath "$.entries.System.data.Version" != "Unknown"
jsonpath "$.entries.System.data.Features" includes "SwaggerUI"
jsonpath "$.entries.System.data.Features" includes "ScalarUI"
jsonpath "$.entries.System.data.Features" includes "UIModule"
jsonpath "$.entries.System.data.Features" includes "ServiceModule"
jsonpath "$.entries.System.data.Features" includes "DebugEndpoint"

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
