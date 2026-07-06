#!/usr/bin/env bash
# Regenerates the _index.md manifest for each .agents/ content dir: docs, plans, memory.
# Each index lists every .md (recursively), grouped by area (the first path segment under the dir;
# root-level files go under "General"), with a one-line description.
# Description source, in order: `description:` frontmatter -> first `# ` heading -> filename.
# Run from the repo root: ./config/agents-index.sh

set -euo pipefail

# describe <file> -> single-line description (pipes escaped for markdown tables).
# `description:` is read ONLY from the front-matter block (--- ... ---) so example lines
# inside code fences don't leak in; falls back to the first `# ` heading, then the filename.
describe() {
  local file="$1" desc
  desc=$(awk 'NR==1 && $0=="---"{f=1;next} f && $0=="---"{exit}
              f && /^description:/{sub(/^description:[[:space:]]*/,"");print;exit}' "$file")
  [[ -z "$desc" ]] && desc=$(grep -m1 "^# " "$file" | sed 's/^#[[:space:]]*//')
  [[ -z "$desc" ]] && desc="${file##*/}"
  printf '%s' "${desc//|/\\|}"
}

# title <group> -> display heading
title() {
  case "$1" in
    api) echo "API" ;;
    lib) echo "Lib" ;;
    vipaq) echo "ViPaq" ;;
    *) printf '%s' "$(tr '[:lower:]' '[:upper:]' <<< "${1:0:1}")${1:1}" ;;
  esac
}

# generate_index <dir> <label> <blurb>
generate_index() {
  local DIR="$1" LABEL="$2" BLURB="$3"
  local INDEX_FILE="$DIR/_index.md"
  [[ -d "$DIR" ]] || { echo "skip $DIR (missing)"; return; }

  {
    echo "---"
    echo "description: Manifest of every file under $DIR, grouped by area. Regenerate with config/agents-index.sh."
    echo "---"
    echo ""
    echo "# $LABEL"
    echo ""
    echo "$BLURB"
  } > "$INDEX_FILE"

  # List every .md relative to DIR, minus the index itself and this dir's own guide README.
  # Nested READMEs (api/README.md, vipaq/README.md, ...) are real content and stay.
  mapfile -t files < <(find "$DIR" -name "*.md" ! -name "_index.md" | sed "s#^$DIR/##" | grep -vx 'README.md' | sort)

  # Discover groups: "General" = root-level files, else first path segment. General first, rest sorted.
  declare -A seen; local groups=() ordered=() rel grp g
  for rel in "${files[@]}"; do
    if [[ "$rel" == */* ]]; then grp="${rel%%/*}"; else grp="General"; fi
    if [[ -z "${seen[$grp]:-}" ]]; then seen[$grp]=1; groups+=("$grp"); fi
  done
  for g in "${groups[@]}"; do [[ "$g" == "General" ]] && ordered+=("$g"); done
  while read -r g; do ordered+=("$g"); done < <(printf '%s\n' "${groups[@]}" | grep -v '^General$' | sort || true)

  for grp in "${ordered[@]}"; do
    {
      echo ""
      echo "## $(title "$grp")"
      echo ""
      echo "| File | Description |"
      echo "|---|---|"
    } >> "$INDEX_FILE"
    for rel in "${files[@]}"; do
      if [[ "$grp" == "General" ]]; then
        [[ "$rel" == */* ]] && continue
      else
        [[ "$rel" != "$grp/"* ]] && continue
      fi
      echo "| [$rel]($rel) | $(describe "$DIR/$rel") |" >> "$INDEX_FILE"
    done
  done

  echo "Regenerated $INDEX_FILE"
}

generate_index ".agents/docs" "Agent Docs Index" \
"Every doc in \`.agents/docs/\`, grouped by area. Scan for your topic, then read that file — do not work
from this summary. Task-based entry points (\"I want to add a v4 endpoint\") are in the \"Common Tasks\"
table of [README.md](README.md)."

generate_index ".agents/plans" "Agent Plans Index" \
"Every plan in \`.agents/plans/\` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands."

generate_index ".agents/memory" "Agent Memory Index" \
"Every memory in \`.agents/memory/\`, grouped by area. Durable facts with no home in a doc or plan —
conventions, decisions, gotchas. See [README.md](README.md) for when and how to add one."
