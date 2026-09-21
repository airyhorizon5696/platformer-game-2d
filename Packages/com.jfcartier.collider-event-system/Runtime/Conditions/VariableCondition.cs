using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Compares a Variable asset's current value against a threshold.
    /// </summary>
    public sealed class VariableCondition : ConditionBase
    {
        [Tooltip("The Variable asset to check.")]
        public Variable targetVariable;

        public ComparisonOperator operatorValue = ComparisonOperator.GreaterThan;
        public TextComparison textOperator = TextComparison.Equals;

        public float floatThreshold;
        public int intThreshold;
        public bool boolThreshold;
        public string stringThreshold;

        public override bool Evaluate()
        {
            switch (targetVariable)
            {
                case FloatVariable f:
                    return ComparisonUtility.Compare(f.RuntimeValue, floatThreshold, operatorValue);

                case IntVariable i:
                    return ComparisonUtility.Compare(i.RuntimeValue, intThreshold, operatorValue);

                case BoolVariable b:
                    // A bool only has two states, so this is always a plain equality check - Operator
                    // (Greater Than/Less Than/...) isn't shown in the Inspector for Bool Variables.
                    return b.RuntimeValue == boolThreshold;

                case StringVariable s:
                    // Alphabetical ordering (Greater Than/Less Than) rarely means anything for named
                    // states, so text gets its own Equals/Not Equals/Starts With/Ends With choice.
                    return ComparisonUtility.Compare(s.RuntimeValue, stringThreshold, textOperator);

                default:
                    return false;
            }
        }
    }
}
