# Unity CLI + Pipeline (agent runbook)

Use this when editing **First Project** through a **running Unity Editor** and the **`unity` CLI** (`com.unity.pipeline`). For repo layout and Play-mode one-liners, see [AGENTS.md](../AGENTS.md).

## Shell setup (every agent terminal)

```bash
source ~/.unity/env
export UNITY_PROJECT_PATH="/Users/johnrake/Development/unity/First Project"
```

Agent sandboxes often omit `~/.unity/bin` from `PATH` and **block loopback** to the Pipeline port. Allow network when calling `unity command …`, or run CLI from the user’s terminal.

**Completion check:** `unity status` lists the project as `ready` with a port before mutating the project.

## Command names (common mistakes)

| Wrong | Use |
|-------|-----|
| `unity play` | `unity command editor.playmode start` |
| `editor_play` | `unity command editor.playmode start` |
| `transform.set_transform` | `unity command gameobject.set_transform` |
| `scene.save_all` | `unity command scene.save` |
| Pipeline while in Play | `unity command editor.playmode stop` before `scene.open` / hierarchy edits |
| `component.add` with a new script type | `unity command script.attach --script Assets/…/Foo.cs` after compile |

Discover names: `unity command --query <term>` or `unity command --json --query set_transform`.

## Adding C# scripts

**Preferred:** let Unity own import metadata.

1. `unity command asset.create_folder --path Scripts` if the folder is not in the AssetDatabase yet.
2. Either `unity command script.create --name MyBehaviour --path Scripts/MyBehaviour.cs` **or** write `MyBehaviour.cs` on disk **without** creating a `.meta` by hand.
3. If the file already exists but is ignored, delete the bad `.meta` and run in the Editor (via `script.eval`):  
   `AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate)`  
   Confirm with `LoadAssetAtPath<MonoScript>("Assets/Scripts/MyBehaviour.cs") != null`.
4. `unity command script.recompile` → poll `unity command script.recompile_status` until `completed` / `up_to_date`.
5. `unity command script.attach --target <handle> --script Assets/Scripts/MyBehaviour.cs`.

On failure, read **`unity command console.get --level error --tail 20`**. Invalid `.meta` guids show up here immediately.

**Do not** trust `asset.read_text` alone — it reads the file from disk and does not prove the AssetDatabase imported the script.

## Scenes and hierarchy

Ordered recipe:

1. `unity command scene.create --path Assets/Scenes/MyScene.unity` or `scene.open --path …`
2. `unity command gameobject.create` (primitives: `--primitive plane|cube|…`)
3. `unity command gameobject.set_transform` / `component.add` / `gameobject.set_parent`
4. **`unity command scene.save`** before `scene.open` on another scene or before assuming work persisted

Attach behaviours with **`script.attach`**, then wire references with **`component.set_field`** when needed.

### EventSystem and Input System

Projects on **Input System only** (`activeInputHandler: 1` in `ProjectSettings/ProjectSettings.asset`) must not use **`StandaloneInputModule`** on the scene **EventSystem** — use **`InputSystemUIInputModule`** instead. Symptom in Play mode: `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class…`

```bash
unity command component.remove --target '{"hierarchyPath":"/EventSystem"}' --type StandaloneInputModule
unity command component.add --target '{"hierarchyPath":"/EventSystem"}' --type InputSystemUIInputModule
unity command scene.save
```

## Play mode

```bash
unity command scene.open --path Assets/Scenes/Playground.unity
unity command --timeout 120 editor.playmode start
```

Increase `--timeout` while the Editor is compiling. Stop with `editor.playmode stop`.

## Local guardrail

From repo root:

```bash
./scripts/check-unity.sh
```

See [CODING_STANDARDS.md](../CODING_STANDARDS.md) for review rules.
