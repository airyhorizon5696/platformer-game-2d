using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Continuously checks its Conditions from the moment the scene starts - no zone, no Collider needed.
    /// Use this for things that don't happen at a place (e.g. "wait until the player has 3 keys").
    /// For a physical zone the player walks into, use ColliderEvent instead.
    /// </summary>
    [AddComponentMenu("Collider Event System/Condition Watcher")]
    public sealed class ConditionWatcher : ColliderEventBase
    {
        // No physical exit event exists for a watcher, so re-check the condition fold while waiting to
        // exit, and treat it going back to false as the exit trigger.
        protected override bool UseConditionFalseAsExit => true;

        // Unlike ColliderEvent (which requires physically leaving and re-entering the zone before it can
        // fire again), a Watcher's Conditions are often still true right after firing - e.g. "has 3 keys"
        // stays true. Without this, it would refire every single frame. Wait for the Conditions to go
        // false at least once before allowing another fire.
        private bool m_WaitingForConditionsToClear;

        protected override void Start()
        {
            base.Start();
            BeginChecking();
        }

        protected override void Update()
        {
            if (m_WaitingForConditionsToClear)
            {
                if (!EvaluateConditions()) m_WaitingForConditionsToClear = false;
                return;
            }

            base.Update();
        }

        protected override void OnFiredAndReset()
        {
            // Execute Exit Actions already has its own false-triggered exit (UseConditionFalseAsExit,
            // handled by the base class) - by the time this runs for that mode, the Conditions have
            // already just gone false, so there's nothing extra to wait for. Every other After Trigger
            // mode needs the gate above.
            if (afterTrigger != AfterTrigger.ExecuteExitActions)
            {
                m_WaitingForConditionsToClear = true;
            }
        }
    }
}
