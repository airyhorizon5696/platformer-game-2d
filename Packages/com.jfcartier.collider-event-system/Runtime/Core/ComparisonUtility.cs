using UnityEngine;

namespace ColliderEventSystem
{
    public static class ComparisonUtility
    {
        public static bool Compare(float current, float target, ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.GreaterThan: return current > target;
                case ComparisonOperator.GreaterThanOrEqual: return current >= target;
                case ComparisonOperator.EqualTo: return Mathf.Approximately(current, target);
                case ComparisonOperator.LessThanOrEqual: return current <= target;
                case ComparisonOperator.LessThan: return current < target;
                default: return false;
            }
        }

        public static bool Compare(int current, int target, ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.GreaterThan: return current > target;
                case ComparisonOperator.GreaterThanOrEqual: return current >= target;
                case ComparisonOperator.EqualTo: return current == target;
                case ComparisonOperator.LessThanOrEqual: return current <= target;
                case ComparisonOperator.LessThan: return current < target;
                default: return false;
            }
        }

        public static bool Compare(string current, string target, TextComparison comparison)
        {
            current ??= string.Empty;
            target ??= string.Empty;

            switch (comparison)
            {
                case TextComparison.Equals: return current == target;
                case TextComparison.NotEquals: return current != target;
                case TextComparison.StartsWith: return current.StartsWith(target, System.StringComparison.Ordinal);
                case TextComparison.EndsWith: return current.EndsWith(target, System.StringComparison.Ordinal);
                default: return false;
            }
        }
    }
}
