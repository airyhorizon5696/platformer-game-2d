namespace ColliderEventSystem
{
    /// <summary>
    /// Used instead of ComparisonOperator for text: "Greater Than"/"Less Than" (alphabetical ordering)
    /// almost never means anything useful when comparing named states like "Locked" or "GameOver".
    /// </summary>
    public enum TextComparison
    {
        Equals,
        NotEquals,
        StartsWith,
        EndsWith,
    }
}
