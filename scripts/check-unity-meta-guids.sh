#!/usr/bin/env bash
# Validates Unity .meta guid lines under committed Assets/ trees.
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
errors=0

while IFS= read -r -d '' meta; do
  guid_line="$(grep -E '^guid: ' "$meta" | head -1 || true)"
  if [[ -z "$guid_line" ]]; then
    continue
  fi
  guid="${guid_line#guid: }"
  if [[ ! "$guid" =~ ^[0-9a-f]{32}$ ]]; then
    echo "invalid guid in $meta: '$guid' (must be exactly 32 lowercase hex digits)" >&2
    errors=$((errors + 1))
  fi
done < <(find "$root" -path '*/Assets/*' -name '*.meta' -print0 2>/dev/null)

if [[ "$errors" -gt 0 ]]; then
  echo "meta guid check failed ($errors file(s))" >&2
  exit 1
fi

echo "meta guid check passed"
