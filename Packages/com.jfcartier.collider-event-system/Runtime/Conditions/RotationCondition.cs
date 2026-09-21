using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Compares a Transform's rotation on one axis, in degrees, against a threshold value. Useful for
    /// things like "this door has swung open past 80 degrees" or "this object has tipped over".
    /// </summary>
    public sealed class RotationCondition : ConditionBase
    {
        public enum Axis
        {
            X,
            Y,
            Z,
        }

        public enum RotationSpace
        {
            World,
            Self,
            RelativeToTransform,
        }

        [Tooltip("The Transform to check.")]
        public Transform target;

        [Tooltip("World measures the Transform's world rotation (includes any parent's rotation). Self " +
                 "measures its local rotation only - the same numbers shown on the Transform component " +
                 "itself. Relative To Transform measures its rotation relative to another Transform you pick.")]
        public RotationSpace space = RotationSpace.World;

        [Tooltip("Used when Space is Relative To Transform.")]
        public Transform relativeTo;

        public Axis axis = Axis.Y;

        public ComparisonOperator operatorValue = ComparisonOperator.GreaterThan;

        public float value;

        public override bool Evaluate()
        {
            if (target == null) return false;

            return ComparisonUtility.Compare(GetCurrentAngle(), value, operatorValue);
        }

        private float GetCurrentAngle()
        {
            Vector3 euler;
            switch (space)
            {
                case RotationSpace.Self:
                    euler = target.localEulerAngles;
                    break;

                case RotationSpace.RelativeToTransform:
                    euler = relativeTo != null
                        ? (Quaternion.Inverse(relativeTo.rotation) * target.rotation).eulerAngles
                        : target.eulerAngles;
                    break;

                default:
                    euler = target.eulerAngles;
                    break;
            }

            float raw;
            switch (axis)
            {
                case Axis.X: raw = euler.x; break;
                case Axis.Y: raw = euler.y; break;
                default: raw = euler.z; break;
            }

            // eulerAngles is always in [0, 360) - a small negative rotation reads as e.g. 355, not -5,
            // which would satisfy "Greater Than 80" even though it barely rotated (the wrong way, at
            // that). Normalize to (-180, 180] so the comparison matches what it actually looks like.
            return Mathf.DeltaAngle(0f, raw);
        }
    }
}
