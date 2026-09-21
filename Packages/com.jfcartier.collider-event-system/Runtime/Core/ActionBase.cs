using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Base class for all Actions. Inherit from this to create a new action type -
    /// it will automatically appear in the "Add Action" list in the Inspector.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class ActionBase : MonoBehaviour
    {
        /// <summary>
        /// If true, Execute(GameObject) is called instead of Execute() so this action can use the
        /// GameObject that triggered the zone. Only meaningful under ColliderEvent, not ConditionWatcher.
        /// </summary>
        public virtual bool RequiresCollisionObjectData => false;

        /// <summary>
        /// How long (in seconds) this action takes to finish. Used to delay AfterTrigger until the
        /// longest-running action among all Actions has completed. Leave as 0 for instant actions.
        /// </summary>
        public virtual float Duration => 0f;

        /// <summary>
        /// Runs this action.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Same as Execute(), but with access to the GameObject that triggered the zone.
        /// Only called when RequiresCollisionObjectData is true.
        /// </summary>
        public virtual void Execute(GameObject collidingObject) => Execute();

        /// <summary>
        /// Called once when the game starts. Use this for one-time setup.
        /// </summary>
        public virtual void OnAwake() { }

        /// <summary>
        /// Called if this action is still animating/running (Duration > 0) when Exit Actions fire early.
        /// Override to stop any coroutine or in-progress work this action started.
        /// </summary>
        public virtual void CancelExecution() { }

        /// <summary>
        /// Called when the owning ColliderEvent/ConditionWatcher resets after firing (AfterTrigger.DoNothing).
        /// </summary>
        public virtual void ResetState() { }

        /// <summary>
        /// Optional inline validation shown in the Inspector as a warning box. Return true and set message
        /// when something required is missing (e.g. an empty reference field).
        /// </summary>
        public virtual bool TryGetValidationWarning(out string message)
        {
            message = null;
            return false;
        }
    }
}
