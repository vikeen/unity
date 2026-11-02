#!/usr/bin/env bash
# Fail when an Input-System-only Unity project still uses legacy UGUI StandaloneInputModule.
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
errors=0

while IFS= read -r settings; do
  [[ -f "$settings" ]] || continue
  if ! grep -qE '^  activeInputHandler: 1$' "$settings"; then
    continue
  fi

  project_root="$(cd "$(dirname "$settings")/.." && pwd)"
  assets="$project_root/Assets"
  [[ -d "$assets" ]] || continue

  while IFS= read -r scene; do
    if grep -q 'StandaloneInputModule' "$scene"; then
      echo "legacy StandaloneInputModule in $scene (project uses Input System only — use InputSystemUIInputModule on EventSystem)" >&2
      errors=$((errors + 1))
    fi
  done < <(find "$assets" -name '*.unity' -print 2>/dev/null)
done < <(find "$root" -path '*/ProjectSettings/ProjectSettings.asset' -print 2>/dev/null)

if [[ "$errors" -gt 0 ]]; then
  echo "input system scene check failed ($errors scene(s))" >&2
  exit 1
fi

echo "input system scene check passed"
