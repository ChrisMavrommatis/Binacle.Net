#!/usr/bin/env bash
# Regenerates .agents/docs/_index.md from the description: frontmatter in each doc.
# Run from the repo root: ./config/docs.sh

set -euo pipefail

DOCS_DIR=".agents/docs"
INDEX_FILE="$DOCS_DIR/_index.md"

cat > "$INDEX_FILE" <<'HEADER'
---
description: Flat manifest of every agent doc — path and one-line description. Regenerate with config/docs.sh.
---

# Agent Docs Index

Full list of every doc in `.agents/docs/`. Read the relevant file for details.

| Doc | Description |
|---|---|
HEADER

# Find all .md files except _index.md itself, sorted, relative to DOCS_DIR
find "$DOCS_DIR" -name "*.md" ! -name "_index.md" | sort | while read -r file; do
  rel="${file#$DOCS_DIR/}"
  desc=$(grep -m1 "^description:" "$file" | sed 's/^description: //')
  if [ -n "$desc" ]; then
    echo "| [$rel]($rel) | $desc |" >> "$INDEX_FILE"
  fi
done

echo "Regenerated $INDEX_FILE"
