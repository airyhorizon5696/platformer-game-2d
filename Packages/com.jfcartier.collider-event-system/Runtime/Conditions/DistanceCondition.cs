using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Compares the distance between two Transforms against a threshold value.
    /// </summary>
    public sealed class DistanceCondition : ConditionBase
    {
        [Tooltip("The first point.")]
        public Transform target;

        [Tooltip("The second point to measure distance to.")]
        public Transform other;

        public ComparisonOperator operatorValue = ComparisonOperator.LessThan;

        public float value;

        public override bool Evaluate()
        {
            if (target == null || other == null) return false;

            float distance = Vector3.Distance(target.position, other.position);
            return ComparisonUtility.Compare(distance, value, operatorValue);
        }
    }
}
