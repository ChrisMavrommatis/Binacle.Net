# Everything this repo builds, loaded by the root justfile as the `build` module.
#
#   just build publish            publish the API into artifacts/binacle-net
#   just build image [version]    publish, then docker build -t binacle-net:<version>
#   just build docs               the documentation site into artifacts/docs
#   just build web                the marketing site into artifacts/web
#
# The two site recipes mirror `just serve docs` / `just serve web`: same site, built once instead of served
# and watched. They are the build half of the pair, which is why they are here rather than in `serve`.
#
# All of them are written to be callable from CI as they stand - nothing interactive, no sudo, no local-only
# paths - so the image a maintainer builds and the image the release workflow ships come from one recipe.
# Creating and opening up the bind-mounted tooling/data and tooling/azurite is not here: those are a
# precondition of running the compose stack, not of building anything, and one of them needs sudo, which CI
# must not run.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# dotnet and docker both print why they failed, so just's own "Recipe failed" line adds nothing. The exit code
# still propagates, so CI still fails.
set no-exit-message := true

api_project := "api/src/Binacle.Net/Binacle.Net.csproj"

# Not a parameter: the Dockerfile hardcodes COPY ["artifacts/binacle-net", "."] and .dockerignore allowlists that
# one path, so publishing anywhere else builds an image without the app in it. A `dir` argument would only
# offer a choice that does not exist.
publish_dir := "artifacts/binacle-net"

# List the recipes
default:
    @just --list build

# One command, not restore + publish --no-restore: publish restores on its own, and nothing caches the restore
# between the two, so the split step only adds a place for the two runtime identifiers to drift apart.
#
# --no-self-contained is spelled out rather than left to the default of a bare --runtime: the runtime comes
# from the base image, which is the whole point, and that has to be readable on the line.
#
# Keep this in step with release-docker-image.yml's publish step - the two must not drift.

# Publish the API framework-dependent for linux-x64, into artifacts/binacle-net
publish:
    dotnet publish {{ api_project }} -c Release -o {{ publish_dir }} --no-self-contained --runtime linux-x64

# The publish is a dependency rather than a check on the folder: `docker build` reads whatever is in
# artifacts/binacle-net, so skipping it when the folder exists is how a stale image gets tagged and shipped.
#
# The version lands in the image twice - as the tag and, via the build arg, as BINACLE_VERSION, which is what
# the running container reports. They come from the same value here for the same reason the release workflow
# feeds both from the metadata step: an image whose reported version disagrees with its tag is unreportable.

# The per-build OCI labels live here, not in the Dockerfile: revision and created change every build, so as
# Dockerfile LABELs fed by ARG they would invalidate the layer cache from that point down. Applied with
# --label they set image-config metadata with no layer and no cache cost. This is the local counterpart to
# metadata-action, so a locally built image carries the same version/revision/created a pushed one does.

# Publish, then build the image [version]
image version="local": publish
    #!/usr/bin/env bash
    set -euo pipefail
    revision="$(git rev-parse HEAD)"
    created="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
    docker build \
      --build-arg VERSION="{{ version }}" \
      --label "org.opencontainers.image.version={{ version }}" \
      --label "org.opencontainers.image.revision=${revision}" \
      --label "org.opencontainers.image.created=${created}" \
      -t "binacle-net:{{ version }}" .

# The two Jekyll sites. Three steps in a fixed order, and skipping any of them produces a site that looks
# built: the asset copy fills the site's gitignored lib/ and media/, webpack compiles _js/ into its gitignored
# js/, and only then does jekyll have everything to assemble. `jekyll build` alone succeeds and ships a site
# with no scripts and no logo - which is exactly the failure worth catching before a deploy rather than after.
#
# --config takes both files: _config.yml holds everything, _config.prod.yml overrides the three values that
# differ off localhost (the site url, the api url, the analytics container). Jekyll merges later files over
# earlier ones, so the order matters.
#
# Output goes to artifacts/<site>, set as `destination` in each _config.yml rather than passed here.
#
# The deploy workflows run these and hand artifacts/<site> straight to the host, so what is built here is what
# gets served. A green run is the deploy, not a rehearsal for one.

# The documentation site into artifacts/docs
docs: (_build-site 'docs' 'sites/docs')

# The marketing site into artifacts/web
web: (_build-site 'web' 'sites/web')

[private]
_build-site name dir:
    #!/usr/bin/env bash
    set -euo pipefail
    # Two arguments because they are two things: the gulp target is keyed on the name, jekyll needs the path.
    # They stopped being the same string when the sites moved under sites/.
    npm run copy-assets-to-{{ name }}
    cd "{{ dir }}"
    npm run build
    bundle exec jekyll build --config _config.yml,_config.prod.yml
