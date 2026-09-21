namespace ColliderEventSystem
{
    /// <summary>
    /// Used for numeric comparisons: DistanceCondition, RotationCondition, and the Global/GameObject
    /// Variable Conditions' Float/Int cases. Bool and String have their own, simpler choices.
    /// </summary>
    public enum ComparisonOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        EqualTo,
        LessThanOrEqual,
        LessThan,
    }
}
