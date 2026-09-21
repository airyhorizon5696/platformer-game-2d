using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Base class for all Conditions. Inherit from this to create a new condition type -
    /// it will automatically appear in the "Add Condition" list in the Inspector.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class ConditionBase : MonoBehaviour
    {
        /// <summary>
        /// How this condition combines with the previous condition in the list. Ignored on the first condition.
        /// </summary>
        public LogicOperator Operator;

        /// <summary>
        /// If true, Evaluate(GameObject) is called instead of Evaluate() so this condition can use the
        /// GameObject that triggered the zone. Only meaningful under ColliderEvent, not ConditionWatcher.
        /// </summary>
        public virtual bool RequiresCollisionObjectData => false;

        /// <summary>
        /// Returns whether this condition is currently met.
        /// </summary>
        public abstract bool Evaluate();

        /// <summary>
        /// Same as Evaluate(), but with access to the GameObject that triggered the zone.
        /// Only called when RequiresCollisionObjectData is true.
        /// </summary>
        public virtual bool Evaluate(GameObject collidingObject) => Evaluate();

        /// <summary>
        /// Called once when the game starts, before any Evaluate calls. Use this for one-time setup.
        /// </summary>
        public virtual void OnAwake() { }

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
