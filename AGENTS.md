# Agent guide

This repository holds **Unity game projects**, not application source for a custom CLI. Each game lives in its own folder at the repo root (see [README.md](README.md)). Generated Unity output (`Library/`, `Temp/`, builds, etc.) is gitignored.

## Active project: First Project

The current game is **[`First Project/`](First%20Project/)** (product name: First Project). Treat this path as the default project root unless the user names another folder.

| | Path |
|---|------|
| Project root | `First Project/` |
| Playground (FPS walkaround) | `First Project/Assets/Scenes/Playground.unity` |
| Template scene | `First Project/Assets/Scenes/SampleScene.unity` |
| Packages | `First Project/Packages/manifest.json` |
| Project settings | `First Project/ProjectSettings/` |

Stack: **URP**, **Input System**, **`com.unity.pipeline`** (CLI-driven Editor). The folder name has a **space** — quote it in shell commands (`"First Project"`).

## Shell (agent terminals)

```bash
source ~/.unity/env
export UNITY_PROJECT_PATH="$(pwd)/First Project"   # from repo root
```

Before Pipeline commands: `unity status` must show the Editor **ready** for this project.

## Unity CLI

Use **`unity`** for installs, builds, and automation; use **`unity command …`** only when the Editor is open with Pipeline connected. Authoritative flags: `unity --help`, `unity <command> --help`.

| Goal | Command |
|------|---------|
| Environment | `unity doctor` |
| Editor connected | `unity status` |
| Open project | `unity open "First Project"` |
| Play in Editor | `unity command --timeout 120 editor.playmode start` |
| Stop Play | `unity command editor.playmode stop` |
| Build | `unity build "First Project"` |

`unity run` is batch-mode, not Game view Play. `unity build` is a player build, not Play mode.

**Pipeline** (new scripts, scenes, hierarchy, attach components): read **[docs/unity-agent-pipeline.md](docs/unity-agent-pipeline.md)** — do not hand-author `.meta` guids; run `./scripts/check-unity.sh` before committing asset changes.

**Review** of Unity assets and scenes: [CODING_STANDARDS.md](CODING_STANDARDS.md).

When adding a new game, create a new subfolder at the repo root and do not commit ignored generated files.
