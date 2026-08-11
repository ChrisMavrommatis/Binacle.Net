# CHANGELOG.md sections, loaded by the root justfile as the `changelog` module.
#
#   just changelog extract 3.0.0      print that version's section
#   just changelog extract Unreleased print the in-progress section
#   just changelog check 3.0.0        exit 1 if the section is missing or empty
#
# The release workflow calls both: `check` as its first gate, `extract` to build the release body. Keeping the
# parsing here means CI and a laptop read the file the same way, and you can preview the exact body you are
# about to publish before you push the tag.

set working-directory := '..'
set no-exit-message := true

changelog := "CHANGELOG.md"

# List the recipes
default:
    @just --list changelog

# Print one version's section, headings promoted to h2 [version|Unreleased]
extract version:
    #!/usr/bin/env bash
    set -euo pipefail
    # Two jobs: find the section, then undo the nesting the file imposes on it.
    #
    # FINDING IT. A section ends at the next VERSION heading, not at the next `## ` - a body may well carry
    # its own `## ` once it has been promoted, and stopping at those would truncate it at its first
    # subheading. So a terminator is a `## ` whose first token parses as `Unreleased` or as semver. Accepts
    # `## 3.0.0`, `## [3.0.0]`, and either with a trailing ` - <date>`, so the file can follow Keep a
    # Changelog exactly without the matcher caring which shape you used.
    #
    # PROMOTING IT. In the file a release is `##` and its own sections are `###`, so the tree nests properly
    # under the single `# Changelog`. A GitHub release body has no such parent - it starts at `##`. So every
    # heading is shifted by whatever it takes to bring the SHALLOWEST one back to `##`, which keeps the
    # relative depth inside the body intact and needs nothing recorded anywhere. Round-trips exactly: what
    # this prints is what was published.
    #
    # Headings inside a fenced code block are left alone - they are content, not structure.
    awk -v want='{{ version }}' '
      function version_token(line,   h, a) {
        h = line; sub(/^## +/, "", h); gsub(/[][]/, "", h)
        split(h, a, / +/)
        if (a[1] == "Unreleased" || a[1] ~ /^[0-9]+\.[0-9]+\.[0-9]+/) return a[1]
        return ""
      }
      BEGIN { min = 99 }
      done { next }
      !f {
        if ($0 ~ /^## / && version_token($0) == want && want != "") f = 1
        next
      }
      {
        fence = ($0 ~ /^[ \t]*```/)
        if (!inside && !fence && $0 ~ /^## / && version_token($0) != "") { done = 1; next }
        if (fence) inside = !inside
        buf[++n] = $0
        if (!inside && !fence && match($0, /^#+ /)) {
          lev[n] = RLENGTH - 1
          if (lev[n] < min) min = lev[n]
        }
      }
      END {
        delta = (min == 99) ? 0 : 2 - min
        for (i = 1; i <= n; i++) {
          if (delta && lev[i]) {
            d = lev[i] + delta; if (d < 1) d = 1
            h = sprintf("%*s", d, ""); gsub(/ /, "#", h)
            print h substr(buf[i], lev[i] + 1)
          } else print buf[i]
        }
      }
    ' {{ changelog }}

# Fail if a version's section is missing or empty [version|Unreleased]
check version:
    #!/usr/bin/env bash
    set -euo pipefail
    notes="$(just changelog::extract '{{ version }}')"
    if [ -z "${notes//[[:space:]]/}" ]; then
        echo "No '## {{ version }}' section in {{ changelog }}." >&2
        echo "Rename [Unreleased] to {{ version }} before tagging." >&2
        exit 1
    fi
    lines=$(printf '%s\n' "$notes" | grep -c '' || true)
    echo "'## {{ version }}' found - ${lines} lines."
