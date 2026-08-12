# The .agents/ manifests, loaded by the root justfile as the `agents` module.
#
#   just agents all                      rewrite all five _index.md
#   just agents generate-index plans     rewrite one
#
# Run it after adding, renaming, moving or re-describing any file under those directories - the index is the
# only thing that makes a file findable without opening every one of them.
#
# Each index lists every .md under its directory (recursively), grouped by area - the first path segment, with
# root-level files under "General" - and one line per file. The description comes from `description:` in the
# front matter, falling back to the first `# ` heading, then the filename.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# The generator already printed why it failed, so just's own "Recipe failed" line adds nothing.
set no-exit-message := true

# List the recipes
default:
    @just --list agents

# The five are listed by hand rather than globbed, so `all` states what it rewrites and a directory only joins
# once someone gives it a heading and a blurb.

# Rewrite every manifest
all:
    #!/usr/bin/env bash
    set -euo pipefail
    for name in docs design plans memory ideas; do
        just agents::generate-index "$name"
    done

# One recipe, one bash process: the generator is a program, not a command line, so it lives in a shebang body
# rather than in a script the recipe would only call. The three tables come first and the rest is the
# algorithm, so adding an index or renaming a group never means reading the generator.

# Rewrite one manifest [docs|design|plans|memory|ideas]
generate-index name:
    #!/usr/bin/env bash
    set -euo pipefail

    # index -> heading. This table is also the validation: an index with no row here is a typo, and a missed
    # typo would write a manifest with an empty heading and look like it worked.
    declare -A headings=(
        [docs]="Agent Docs Index"
        [design]="Agent Design Index"
        [plans]="Agent Plans Index"
        [memory]="Agent Memory Index"
        [ideas]="Agent Ideas Index"
    )

    # index -> the blurb under that heading. The wrapped lines sit flush with the body's own indent on
    # purpose: just strips that indent, so they land at column 0 in the manifest. Indent them to line up with
    # their own row and the manifest gets the leading spaces too.
    declare -A blurbs=(
        [docs]="Every doc in \`.agents/docs/\`, grouped by area. Scan for your topic, then read that file — do not work
    from this summary. Task-based entry points (\"I want to add a v4 endpoint\") are in the \"Common Tasks\"
    table of [README.md](README.md)."
        [design]="Every design record in \`.agents/design/\`, grouped by area. The settled design behind the docs — the
    decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need."
        [plans]="Every plan in \`.agents/plans/\` (recursive), grouped by area. Plans are work not yet done — read the one
    you need, and trim or delete it once the work lands."
        [memory]="Every memory in \`.agents/memory/\`, grouped by area. Durable facts with no home in a doc or plan —
    conventions, decisions, gotchas. See [README.md](README.md) for when and how to add one."
        [ideas]="Every idea in \`.agents/ideas/\`, grouped by area. Ideas are rough and unvetted — read the one you need,
    and move it to \`.agents/plans/\` once it's picked up. See [README.md](README.md) for the conventions."
    )

    # group -> heading, for the groups whose capitalisation we can't guess. Any other group gets its first
    # letter uppercased, which is right for shared, samples, tooling and every slice added since.
    declare -A group_headings=(
        [api]="API"
        [lib]="Lib"
        [vipaq]="ViPaq"
        [ci-cd]="CI/CD"
        [docs-site]="Docs Site"
        [web-site]="Web Site"
    )

    index='{{ name }}'
    heading="${headings[$index]:-}"
    [[ -n "$heading" ]] || { echo "Unknown index '$index'. Use docs, design, plans, memory or ideas." >&2; exit 1; }

    dir=".agents/$index"
    [[ -d "$dir" ]] || { echo "$dir does not exist." >&2; exit 1; }
    index_file="$dir/_index.md"

    # describe <file> -> single-line description (pipes escaped for markdown tables).
    # `description:` is read ONLY from the front-matter block (--- ... ---) so example lines
    # inside code fences don't leak in.
    describe() {
      local file="$1" desc
      desc=$(awk 'NR==1 && $0=="---"{f=1;next} f && $0=="---"{exit}
                  f && /^description:/{sub(/^description:[[:space:]]*/,"");print;exit}' "$file")
      [[ -z "$desc" ]] && desc=$(grep -m1 "^# " "$file" | sed 's/^#[[:space:]]*//')
      [[ -z "$desc" ]] && desc="${file##*/}"
      printf '%s' "${desc//|/\\|}"
    }

    {
      echo "---"
      echo "description: Manifest of every file under $dir, grouped by area. Regenerate with just agents all."
      echo "---"
      echo ""
      echo "# $heading"
      echo ""
      echo "${blurbs[$index]}"
    } > "$index_file"

    # List every .md relative to dir, minus the index itself and this dir's own guide README.
    # Nested READMEs (api/README.md, vipaq/README.md, ...) are real content and stay.
    mapfile -t files < <(find "$dir" -name "*.md" ! -name "_index.md" | sed "s#^$dir/##" | grep -vx 'README.md' | sort)

    # Discover groups: "General" = root-level files, else first path segment. General first, rest sorted.
    declare -A seen; groups=(); ordered=()
    for rel in "${files[@]}"; do
      if [[ "$rel" == */* ]]; then grp="${rel%%/*}"; else grp="General"; fi
      if [[ -z "${seen[$grp]:-}" ]]; then seen[$grp]=1; groups+=("$grp"); fi
    done
    for g in "${groups[@]}"; do [[ "$g" == "General" ]] && ordered+=("$g"); done
    while read -r g; do ordered+=("$g"); done < <(printf '%s\n' "${groups[@]}" | grep -v '^General$' | sort || true)

    for grp in "${ordered[@]}"; do
      {
        echo ""
        echo "## ${group_headings[$grp]:-${grp^}}"
        echo ""
        echo "| File | Description |"
        echo "|---|---|"
      } >> "$index_file"
      for rel in "${files[@]}"; do
        if [[ "$grp" == "General" ]]; then
          [[ "$rel" == */* ]] && continue
        else
          [[ "$rel" != "$grp/"* ]] && continue
        fi
        echo "| [$rel]($rel) | $(describe "$dir/$rel") |" >> "$index_file"
      done
    done

    echo "Regenerated $index_file"
