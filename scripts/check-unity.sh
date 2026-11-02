#!/usr/bin/env bash
# Run all Unity repo guardrails (also invoked from CI).
set -euo pipefail

dir="$(cd "$(dirname "$0")" && pwd)"

"$dir/check-unity-meta-guids.sh"
"$dir/check-unity-forbidden-paths.sh"
"$dir/check-unity-input-system.sh"

echo "all unity repo checks passed"
