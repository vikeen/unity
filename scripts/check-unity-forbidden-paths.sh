#!/usr/bin/env bash
# Block tracked paths that should never be committed (orphan metas, scratch folders).
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
errors=0

while IFS= read -r path; do
  [[ -n "$path" ]] || continue
  echo "forbidden tracked path: $path (remove from git and add to .gitignore if needed)" >&2
  errors=$((errors + 1))
done < <(git -C "$root" ls-files ':(glob)**/Assets/Temp.meta' 2>/dev/null || true)

if [[ "$errors" -gt 0 ]]; then
  echo "forbidden path check failed ($errors file(s))" >&2
  exit 1
fi

echo "forbidden path check passed"
