# zero - as shipped, no env. One claim: the image boots from nothing and a core route answers. Do NOT grow
# this into a list of 404s - a bare list of 404s is unfalsifiable (a wrong image passes it too).

GET http://localhost:8080/api/v3/presets
HTTP 200

# Read a preset through to its bins by name and count, never dimensions. /api/v4/presets returns 200 with an
# empty dictionary even when nothing bound, so reading through proves Presets.json actually loaded.
GET http://localhost:8080/api/v4/presets
HTTP 200
[Asserts]
jsonpath "$.presets['rectangular-cuboids']" count == 3
jsonpath "$.presets['perfect-cubes']" count == 3
jsonpath "$.presets['sample']" count == 3
