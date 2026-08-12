# minimal - the image as shipped, no env. One claim: it boots from nothing and a core route answers. That single
# 200 proves the entry point ran, the runtime works, the port is right and the non-optional config files were
# found. Do NOT grow this into a list of 404s - a bare list of 404s is unfalsifiable, since a wrong image, a
# container that read no config and a typo'd flag all pass it.

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
