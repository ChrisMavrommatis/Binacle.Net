# prod - a self-hosted deployment: health checks and packing logs on, nothing else. No docs, no UI, no
# /_debug, no ServiceModule. The defining assertion is /_health 200 with an EMPTY Features array, which proves
# health is on AND that nothing else is, in one request. Redirects are off.

# The custom presets are the positive that anchors every 404 below: this preset exists only in the mounted
# file, so reading it back proves the right image started AND that it read config we supplied. A default
# preset would have been there either way.
GET http://localhost:8080/api/v3/presets
HTTP 200

GET http://localhost:8080/api/v4/presets
HTTP 200
[Asserts]
jsonpath "$.presets['smoke-lockers']" count == 2
jsonpath "$.presets['rectangular-cuboids']" not exists

# health on, everything else off. Features can only ever contain SwaggerUI, ScalarUI, UIModule, ServiceModule,
# RateLimiter and DebugEndpoint - HealthChecks and PackingLogs are not features - so empty is exactly right,
# and any module switching itself on would break this line.
GET http://localhost:8080/_health
HTTP 200
[Asserts]
jsonpath "$.status" == "Healthy"
jsonpath "$.entries.System.data.Version" != "Unknown"
jsonpath "$.entries.System.data.Features" count == 0

# no docs - swagger and scalar are off, so the openapi documents are not mounted either
GET http://localhost:8080/swagger/index.html
HTTP 404

GET http://localhost:8080/scalar/
HTTP 404

GET http://localhost:8080/openapi/v3.json
HTTP 404

GET http://localhost:8080/openapi/v4.json
HTTP 404

# no ServiceModule: the auth route is not mounted at all - 404, not the 405 a mounted POST-only route gives
GET http://localhost:8080/api/auth/token
HTTP 404

# and no admin surface to protect
GET http://localhost:8080/api/admin/account/11111111-1111-1111-1111-111111111111
HTTP 404

# no web UI
GET http://localhost:8080/
HTTP 404

# no debug endpoint
GET http://localhost:8080/_debug
HTTP 404
