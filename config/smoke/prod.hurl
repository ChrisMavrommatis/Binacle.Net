# prod - the real production shape: docs + service + health + packing logs on; UI_MODULE and DEBUG_ENDPOINT
# off. The defining assertion is /_debug 404. Redirects are off.

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

# service on and swagger on, so the service document is served
GET http://localhost:8080/openapi/service.json
HTTP 200

# health on: Features positively states swagger, scalar and service are on and ui/debug are off - this anchors
# the 404s below so they cannot pass for the wrong reason.
GET http://localhost:8080/_health
HTTP 200
[Asserts]
jsonpath "$.status" == "Healthy"
jsonpath "$.entries.System.data.Version" != "Unknown"
jsonpath "$.entries.System.data.Features" includes "SwaggerUI"
jsonpath "$.entries.System.data.Features" includes "ScalarUI"
jsonpath "$.entries.System.data.Features" includes "ServiceModule"
jsonpath "$.entries.System.data.Features" not includes "UIModule"
jsonpath "$.entries.System.data.Features" not includes "DebugEndpoint"

# the production security assertion: the request-echoing debug endpoint is not exposed
GET http://localhost:8080/_debug
HTTP 404

# UI module off
GET http://localhost:8080/
HTTP 404

# service on: a token for the seeded admin proves the whole storage path in one request (the startup task wrote
# the account before the port opened, and the endpoint reads it back).
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

# admin route mounted AND protected: 401, not 404 (never mounted) and not 200 (open to the world). Use a real
# GUID - Guid.Empty is rejected as invalid (422) before the lookup.
GET http://localhost:8080/api/admin/account/11111111-1111-1111-1111-111111111111
HTTP 401
