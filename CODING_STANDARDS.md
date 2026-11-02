# Coding standards (review)

Read during **review**, not while exploring or implementing. The implementation agent should follow [AGENTS.md](AGENTS.md) and, when using the Unity CLI against a running Editor, [docs/unity-agent-pipeline.md](docs/unity-agent-pipeline.md).

## Unity asset metadata

- **Never hand-author a `guid:` in a `.meta` file.** Unity requires exactly **32 lowercase hexadecimal characters**. An invalid guid makes Unity **ignore the asset** (often with only a Console error). Fix: delete the bad `.meta`, let the Editor recreate it, or create the asset through the Editor / `unity command script.create` / `asset.create_folder`.
- **Do not commit a partial `.meta`** (for example a file that contains only `fileFormatVersion` and `guid` without the importer block Unity expects). Treat truncated metas as a blocker.
- Before approving PRs that add scripts under `Assets/`, run `./scripts/check-unity-meta-guids.sh` (also enforced in CI).

## C# under Assets

- Prefer **one public behaviour type per script file**, type name matching the file name, so `script attach` can resolve the class after compile.
- After adding or changing scripts, expect **`script recompile`** and polling **`script recompile_status`** until `completed` or `up_to_date` before attaching components or entering Play mode.

## Scenes changed via Pipeline

- **`scene save`** after hierarchy edits. Re-opening a scene without saving drops unsaved Pipeline changes.
- Verify the saved `.unity` on disk contains expected root objects before marking the task done.

## Judgement (no automated substitute)

- Match existing project conventions (URP, Input System, folder layout under `First Project/Assets/`).
- Keep gameplay scripts in `Assets/Scripts/` unless the user asks for a different layout.
