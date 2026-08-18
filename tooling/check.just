# Checks that inspect what the repo produced, loaded by the root justfile as the `check` module.
#
#   just check links                   internal links in both built sites
#   just check links <site>            one of them
#   just check links-external <site>   every link, other people's servers included
#   just check workflows               actionlint over .github/
#
# `workflows` needs no arguments: actionlint finds .github/workflows by itself from the repo root, and follows
# `uses: ./.github/actions/*` into the composite actions.
#
# It reads the built site, never the source. A source href is Liquid - `{{ site.baseurl }}/api` is not a link
# yet - so checking `docs/` would check strings that are not the URLs anyone visits. Build first.
#
# `links` passes --offline, which drops every external link. That is not timidity about the network: an
# external check fails on somebody else's rate limit or outage, and one that goes red for reasons the author
# did not cause stops being read. Internal links are the ones a rename breaks, and they answer the same way
# every time. That is why the external run is a separate recipe rather than a flag, and why CI calls `links`.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# lychee prints every broken link with its file and line, so just's own "Recipe failed" line adds nothing.
# The exit code still propagates, so CI still fails.
set no-exit-message := true

# Named because the config no longer sits in the working directory, where lychee finds one by itself.
# Absolute for the same reason as the openapi paths: the recipe runs from the repo root, and this file does
# not move with it.
lychee_config := justfile_directory() / "tooling" / "lychee.toml"

# List the recipes
default:
    @just --list check

# Internal links in the built sites - both with no argument, or one [site]
links site="":
    #!/usr/bin/env bash
    set -euo pipefail
    sites="{{ site }}"
    [ -n "$sites" ] || sites="docs web"
    for s in $sites; do
        just check _lychee "$s" --offline
    done

# Every link in one site, external ones included - slower, and it can fail on somebody else's outage
links-external site: (_lychee site)

# The workflow and action files: expressions, shell quoting, runner labels, action inputs
workflows:
    #!/usr/bin/env bash
    set -euo pipefail
    # Passed explicitly rather than letting actionlint discover them, so the list printed here is the list it
    # was given. It reads the composite actions too, to check their inputs, but it lints workflow files only.
    # actionlint prints nothing at all when it is happy, which reads like it never ran. Listing the files says
    # what it was handed; silence under that list is the pass.
    files=(.github/workflows/*.yml)
    printf '%s\n' "${files[@]}"
    actionlint -color "${files[@]}"

[private]
_lychee site *flags:
    #!/usr/bin/env bash
    set -euo pipefail
    dir="artifacts/{{ site }}"
    if [ ! -d "$dir" ]; then
        echo "No $dir. Run 'just build {{ site }}' first - this reads the built site." >&2
        exit 1
    fi
    # --root-dir is what resolves a root-relative href like /core-concepts/ against the site root instead of
    # the filesystem root. It has to be absolute; the recipe runs from the repo root, so $PWD is that.
    lychee --config '{{ lychee_config }}' {{ flags }} --root-dir "$PWD/$dir" --no-progress "$dir/**/*.html"
