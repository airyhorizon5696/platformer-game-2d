namespace ColliderEventSystem
{
    /// <summary>
    /// Relates a condition to the previous condition in the list. Ignored on the first condition.
    /// Evaluated strictly left-to-right in list order, e.g. "A Or B And C" reads as "(A Or B) And C".
    /// </summary>
    public enum LogicOperator
    {
        And,
        Or,
    }
}
