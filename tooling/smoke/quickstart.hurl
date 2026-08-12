# quickstart - SWAGGER_UI + SCALAR_UI + UI_MODULE (the README docker run). Docs and the web UI serve; with
# service, health and debug off, those surfaces are 404. Redirects are off (hurl default), so request the real
# pages: /swagger/ 301s to index.html, /scalar 302s to /scalar/.

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

GET http://localhost:8080/
HTTP 200

# service is off, so the service OpenAPI document is never registered
GET http://localhost:8080/openapi/service.json
HTTP 404

# health and debug are off - both surfaces absent
GET http://localhost:8080/_health
HTTP 404

GET http://localhost:8080/_debug
HTTP 404

# service off - the auth route is not mounted at all (404, not the 405 a mounted POST-only route gives)
GET http://localhost:8080/api/auth/token
HTTP 404
