# Checks that inspect what the repo produced, loaded by the root justfile as the `check` module.
#
#   just check links                   internal links in both built sites
#   just check links <site>            one of them
#   just check links-external <site>   every link, other people's servers included
#   just check workflows               actionlint over .github/workflows
#   just check actions                 the .github/actions manifests, which actionlint cannot read
#
# Both name the files they were handed and count them at the end. actionlint and grep say nothing when they
# pass, so that count is the only thing separating a clean run from one that never started.
#
# It reads the built site, never the source. A source href is Liquid - `{{ site.baseurl }}/api` is not a link
# yet - so checking `sites/docs/` would check strings that are not the URLs anyone visits. Build first.
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
    [ -n "$sites" ] || sites="docs demo"
    for s in $sites; do
        just check _lychee "$s" --offline
    done

# Every link in one site, external ones included - slower, and it can fail on somebody else's outage
links-external site: (_lychee site)

# The workflow files: expressions, shell quoting, runner labels, action inputs
workflows:
    #!/usr/bin/env bash
    set -euo pipefail
    files=(.github/workflows/*.yml)
    echo "checking ${#files[@]} workflows in .github/workflows"
    printf '  %s\n' "${files[@]##*/}"
    # actionlint prints its own errors and exits non-zero, so set -e stops us before the all-clear below.
    actionlint -color "${files[@]}"
    echo "${#files[@]} workflows, no errors"

# The composite action manifests, which actionlint cannot read
actions:
    #!/usr/bin/env bash
    set -euo pipefail
    files=(.github/actions/*/action.yml)
    names=("${files[@]%/action.yml}")
    echo "checking ${#files[@]} actions in .github/actions"
    printf '  %s\n' "${names[@]##*/}"
    # By hand because actionlint treats every file as a workflow. A vars or secrets expression anywhere in a
    # manifest, descriptions included, fails the action load on the runner.
    # grep prints the offending file, line and text itself; the line below says what to do about it.
    if grep -nE --color=auto '\$\{\{[^}]*\b(vars|secrets)\.' "${files[@]}"; then
        echo "take it as an input instead - neither context exists inside an action" >&2
        exit 1
    fi
    echo "${#files[@]} actions, no errors"

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
