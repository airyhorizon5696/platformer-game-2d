# Known issues / potential fixes

Things that are known limitations but aren't scheduled yet - either low impact, not reproduced, or
waiting on a concrete repro before deciding on the right fix.

## `Transform Action`: concurrent suspensions on the same CharacterController can re-enable it early

Introduced alongside the `CharacterController` suspend/restore fix in 0.4.7
([TransformAction.cs](Runtime/Actions/TransformAction.cs)).

**Scenario:** two `Transform Action`s target the same GameObject's `CharacterController` at the same
time (e.g. one animating Position over 2s, another animating Rotation over 1s, both fired by the same
trigger). Only the first to run actually disables the controller and records itself as owner
(`m_SuspendedController`) - the second finds it already disabled and doesn't take ownership. If the
first one is cancelled early (e.g. `Exit Actions` fire before its animation finishes), it re-enables the
controller while the second Action is still animating, so the second Action starts fighting the
`CharacterController` again mid-animation - the same symptom the 0.4.7 fix addressed, just re-appearing
for the second Action in this specific overlap.

**Why not fixed yet:** no concrete repro has come up in practice - this requires two `Transform Action`s
on the *same* target with *different* durations *and* an early cancellation, which is a fairly narrow
combination. Fixing it "for real" needs a shared per-component suspend refcount (increment on suspend,
decrement on restore, only re-enable at zero) instead of the current single-owner flag, which is a bit
more machinery than the fix seemed to warrant without evidence it happens.

**If it needs fixing:** replace the `CharacterController m_SuspendedController` instance field with a
static `Dictionary<CharacterController, int>` refcount shared across all `Transform Action` instances,
incremented in `SuspendRigidbody` and decremented in `RestoreRigidbody`, only setting
`controller.enabled = true` when the count reaches zero.
