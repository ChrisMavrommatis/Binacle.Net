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
