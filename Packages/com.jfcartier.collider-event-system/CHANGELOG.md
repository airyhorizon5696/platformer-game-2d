# Changelog

All notable changes to this package are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [0.4.7]

### Fixed

- `Transform Action` only suspended a `Rigidbody` while moving a target, not a `CharacterController` -
  a `CharacterController` left enabled can silently reject or immediately undo a direct position/rotation
  change, making the Action appear to do nothing on a target like a Third Person Controller's capsule.
  It's now disabled for the duration of the move (instant or animated) and re-enabled once it finishes,
  matching how a Rigidbody is already handled.

## [0.4.6]

### Fixed

- A Condition/Action's `HideInInspector` flag (which keeps it from also drawing as its own raw
  component block, since it's always shown inline in the Conditions/Actions list instead) could get
  silently dropped by certain Prefab operations, making it reappear as a duplicate "(Script)" block
  underneath the list. The flag is now reapplied automatically whenever the Inspector is opened, so
  the duplicate block can no longer stick around.

## [0.4.5]

### Added

- Offline PDF documentation (`Documentation/`), covering installation, every Condition and Action,
  Variables, and known gotchas - required for Asset Store submission.

### Changed

- Quick Start now leads with dragging one of the ready-made trigger prefabs into the scene instead of
  building a trigger from scratch; the manual setup (Collider + Is Trigger + Collider Event) is now the
  secondary path, for adding a trigger to an existing GameObject.

### Fixed

- `ColliderEvent`'s capsule gizmo mesh cache is now a per-instance field instead of a shared static one,
  so it no longer needs an explicit Play Mode reset. No behaviour change - purely satisfies static-analysis
  tooling (e.g. the Asset Store Validator) that flags any mutable static field regardless of scope.

## [0.4.4]

### Fixed

- Demo sample's `Material` trigger was set to `ExecuteExitActions` instead of `Destroy` like every other
  trigger in the scene, so entering its zone didn't make it disappear the way the others do. Now uses
  `Destroy`, for consistency across the Demo.

## [0.4.3]

### Fixed

- `Transform Action`'s Additive rotation could produce no visible motion at all when the configured delta
  was a multiple of 360 on one axis (e.g. a "spin 360" effect) - `Quaternion.Euler(0, 0, 360)` is the same
  orientation as no rotation at all, so Slerping straight to it (as 0.1.8's fix for Additive drift did)
  never moved. Additive now reconstructs the rotation from the raw delta at each frame's progress instead
  of Slerping toward a fixed endpoint, so a full turn (or several) animates as an actual visible spin.

## [0.4.2]

### Changed

- Demo sample updated to demonstrate Hint Material (new `Demo Hint` material, reusing the existing
  cross-pipeline "Demo Transparent" Shader Graph).

## [0.4.1]

### Changed

- Every Target Mode field (Transform, Material, GameObject, Rigidbody, Animation, Instantiate Prefab
  Actions, and Hint Material) now defaults to **Specific Object** instead of Entering Objects, for
  consistency across the package. Only affects newly added components - existing ones keep whatever
  they're already set to.

## [0.4.0]

### Changed

- **Hint Material moved from Condition to Collider Event / Condition Watcher.** It reflects the combined
  Condition list result (respecting each Condition's And/Or), not any single Condition's own value - with
  more than one Condition, a per-Condition Hint Material would have shown/hidden based on that Condition
  alone, out of step with when Actions actually run. Behaviour is otherwise identical: applied while the
  Conditions aren't all met, restored once they are or once the zone is left first.

## [0.3.0]

### Added

- Every Condition now has an optional **Hint Material**: applied to a target while the Condition is
  false, and automatically restored once it becomes true (or once the zone is left before it does) -
  useful as a hint that an interaction is available (e.g. an Input Condition highlighting the object you
  can press E on). Shares its apply/restore logic with Material Action's Restore Original mode, so the
  two never disagree about what the "original" material was if they touch the same Renderer.

## [0.2.0]

### Added

- `Material Action` has a new **Mode**: **Apply** (previous behaviour) or **Restore Original**. Apply now
  remembers the Renderer's materials the first time it runs; Restore Original puts them back without
  needing a second, hand-picked "original" material - useful for a temporary highlight while a zone's
  Conditions are still pending (e.g. Action = Apply a highlight material on enter, Exit Action = Restore
  Original), all on a Collider Event with no Conditions of its own so it fires immediately.

## [0.1.10]

### Fixed

- Trigger prefabs (`Prefabs/Trigger Cube`, `Trigger Sphere`, `Trigger Capsule`) referenced Unity's built-in
  default material (the Standard shader), which shows up pink on a URP project - now used a custom Shader
  Graph material (`Materials/Trigger Zone`, targeting both Universal and Built-In) instead, so enabling the
  Mesh Renderer for a visible zone works out of the box regardless of render pipeline.

## [0.1.9]

### Fixed

- Condition/Action list rows now have left padding so their content (foldout, fields) doesn't sit
  underneath the ReorderableList's drag handle.

## [0.1.8]

### Fixed

- `Transform Action`'s **Additive** rotation mode no longer accumulates via Euler angle arithmetic
  (`start + delta`, then writing back through `eulerAngles`). Since `eulerAngles` isn't a stable
  round-trip representation once more than one axis is involved, this could silently drift or even
  cancel out from one run to the next (e.g. alternating +10/-10 instead of accumulating). Now composes
  `Quaternion`s directly and animates with `Slerp` instead of a per-axis `Lerp` on Euler angles.

## [0.1.7]

### Fixed

- `ColliderEvent`: avoided two small GC allocations found by Project Auditor - the 3-argument
  `Mathf.Max` call in the Sphere gizmo path (which allocates a `float[]` via its `params` overload) and
  `new string[0]` for the default Required Tags value (now `Array.Empty<string>()`).

## [0.1.6]

### Fixed

- `ColliderEvent`'s cached capsule gizmo mesh is now explicitly cleared on every Play Mode entry
  (`[InitializeOnEnterPlayMode]`), so it can never carry a stale reference under Fast Enter Play Mode /
  disabled domain reload.

## [0.1.5]

### Added

- Demo's notification sound now also ships as a lossless `.wav`, alongside the original `.mp3` (same file
  name), per Asset Store guidelines against lossy-only audio.

## [0.1.4]

### Fixed

- Demo sample: a misconfigured Shader Graph material, a mislabeled scene object, and a non-functional
  prefab reference, all in the demo scene itself.

## [0.1.3]

### Changed

- Demo sample's on-screen label switched from TextMeshPro to the legacy UI Text, to avoid the one-time
  "Import TMP Essentials" prompt Unity shows the first time TextMeshPro is used in a project - no import
  step of any kind is needed to try the Demo now. Uses the same bundled Inter font (a plain .ttf, no
  TMP-specific font asset needed).

## [0.1.2]

### Fixed

- Demo sample's materials (Demo Dark, Demo Grid, Demo Condition Met, Demo Transparent) now use custom
  Shader Graphs targeting both URP and Built-In, instead of a URP-only shader - no more pink materials or
  running the Render Pipeline Converter on a Built-In project.
- Demo sample now bundles its own font (Inter, SIL Open Font License) instead of referencing the Editor's
  TMP Essentials font, which doesn't exist in a project that has never imported TextMeshPro before.

## [0.1.1]

### Fixed

- `LICENSE` and `CHANGELOG.md` were missing `.meta` files, so Unity ignored them (with a console warning)
  when the package was installed via git URL.

## [0.1.0]

### Added

- **Collider Event** - zone-based trigger built on any native Collider.
- **Condition Watcher** - always-on equivalent with no physical zone, for state-based waits (e.g. "player
  has 3 keys").
- **Conditions**: Looking At, Input, Variable, Distance, Rotation - foldable left-to-right with an And/Or
  operator per condition.
- **Actions**: Animation, Audio, Scene, GameObject, Material, Rigidbody, Transform, Variable, Invoke
  Events, Instantiate Prefab.
- **Variables**: Float/Int/Bool/String assets, optionally persisted to disk between play sessions.
- **Target Mode** (Entering Objects / Specific Object) on every Action that acts on a GameObject, with
  Entering Objects automatically unavailable on a Condition Watcher (no entering object to give it).
- Custom Inspector tooling throughout: grouped "Target"/"Effect" sections, conditional field visibility,
  a "+" button to create a Variable inline, and inline validation warnings for non-obvious silent failures.
- Starter prefabs (`Prefabs/`) for Cube, Sphere, and Capsule trigger zones.
- **Demo** sample scene demonstrating Conditions, Actions, and Variables together.
