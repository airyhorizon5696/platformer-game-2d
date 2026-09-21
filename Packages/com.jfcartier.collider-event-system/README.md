# Collider Event System

Attach ordered **Conditions** and **Actions** to a GameObject to build zone triggers and always-on
condition watchers, without writing code.

## Components

- **Collider Event** - add alongside any native Collider (mark it "Is Trigger"). Waits for something to
  enter the zone, then checks the Conditions. Optionally restrict which Tags can trigger it - leave every
  box unchecked to let anything with a Collider trigger it.
- **Condition Watcher** - no Collider needed. Checks the Conditions continuously from the moment the
  scene starts, for things that don't happen at a place (e.g. "wait until the player has 3 keys").

## Quick start

1. Drag one of the ready-made prefabs (`Trigger Cube`, `Trigger Sphere`, or `Trigger Capsule` - in
   this package's `Prefabs` folder) into your scene.
2. Under Conditions, click "Add Condition" and pick one (e.g. Distance).
3. Under Actions, click "Add Action" and pick one (e.g. Invoke Events).
4. Enter Play Mode and walk into the zone.

To add a trigger to a GameObject of your own instead: add a Collider (e.g. Box Collider), tick "Is
Trigger", then add **Collider Event** yourself.

## Conditions

Looking At, Input, Variable, Distance, Rotation.

Each condition after the first has an **And/Or** selector relating it to the one above it, read strictly
top to bottom (e.g. "A Or B And C" reads as "(A Or B) And C").

## Actions

Animation, Audio, Scene, GameObject, Material, Rigidbody, Transform, Variable, Invoke Events, Instantiate
Prefab.

## Variables

Instead of typing a string key, create a Variable asset (**Assets > Create > Collider Event System >
Variables**) and drag it into a Variable Condition or Variable Action - or click the "+" next to the
field to create one on the spot. Toggle **Persistent** on a Variable to have it saved to disk and
reloaded automatically between play sessions.

Note: a Variable asset is shared - every GameObject that references the same one reads/writes the same
value. There's currently no built-in way to give each instance of a prefab (e.g. each enemy) its own
independent value like HP without writing a script for it.

## Base options

- **Hold Time** - how many seconds the Conditions must stay true, without interruption, before Actions run.
- **After Trigger** - what happens once Actions have run (deactivate, destroy, do nothing and reset, or
  wait and run Exit Actions once the Conditions/zone go inactive again).
- **Hint Material** - optional. A material applied to a target while the Conditions above aren't all met
  yet, and automatically restored once they are (or once the zone is left before they are) - useful as a
  hint that an interaction is available, e.g. highlighting the object you can press E on until you
  actually press it. Reflects the combined Condition list result (respecting And/Or), not any single
  Condition on its own.

## Prefabs

`Prefabs/` has ready-made trigger zones (Trigger Cube, Trigger Sphere, Trigger Capsule) - each a Collider
set to "Is Trigger" with a Collider Event already attached, ready to drag into a scene as shown in Quick
start.

## Sample

Import **Demo** from this package's page in the Package Manager window for a walkable scene that puts
several Conditions, Actions, and Variables to use together.

Its materials are authored for URP. On a project still using the Built-in Render Pipeline, they'll show up
pink until you run **Window > Rendering > Render Pipeline Converter** (Unity's own built-in fixer) - a
one-time, couple-clicks step, not something wrong with the package itself. The package's actual Conditions
and Actions have no material/shader dependency at all; this only affects the optional Demo sample.

## Gotchas

A few things that aren't bugs, but are easy to trip over:

- **Rotation Condition measures world rotation by default.** If the object you're checking has a rotated
  parent, its world rotation includes the parent's rotation too - not just the number shown on the
  object's own Transform. Switch **Space** to **Self** to compare its local rotation instead.
- **`Transform.eulerAngles` is always 0-360, never negative.** A slight rotation "the wrong way" (e.g. -5°)
  reads as 355°, which can satisfy a "Greater Than" comparison you didn't intend. Rotation Condition
  already normalizes this internally, but keep it in mind if you inspect rotation values elsewhere.
- **Scene Action loading a scene not in Build Settings works in the Editor, but not in a real build.**
  Unity's Editor Play Mode has a convenience fallback that resolves a scene by name across the whole
  project; a built player doesn't have it. Add every scene you load to Build Settings before shipping.
- **Cancel Physics (Transform Action) doesn't pause momentum, it erases it.** A kinematic Rigidbody always
  reports zero velocity. Once restored to non-kinematic after the move finishes, the object resumes at
  rest, not from whatever speed it had before - it won't carry on falling at its old speed, for example.
- **Drag from the Hierarchy, not the Project window, into a Target field.** Dragging a prefab *asset* gives
  you a reference to the asset itself (whose values never change at runtime), not the instance in your
  scene. This is a general Unity gotcha, not specific to this package, but it's an easy mix-up.
- **A Cylinder's Collider is actually a Capsule.** Unity has no native cylinder-shaped collider - the
  default Cylinder primitive gets a Capsule Collider, rounded ends included. The Debug Color gizmo draws
  the real collider shape, so a "cylinder" zone will show (and behave) like a capsule.

## Credits

Initially built starting from the concept behind Alexander Scott's
[Enhanced Trigger Box](https://github.com/alexander-scott/Enhanced-Trigger-Box) - a trigger volume driven by
composable conditions and actions. This package has since diverged into its own architecture (Conditions,
Actions, and Variables as separate composable components, a Condition Watcher for non-physical triggers,
custom Inspector tooling, etc.), but the original idea is worth crediting.
