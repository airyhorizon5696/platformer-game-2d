using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Writes a value to a Variable asset.
    /// </summary>
    public sealed class VariableAction : ActionBase
    {
        [Tooltip("The Variable asset to write to.")]
        public Variable targetVariable;

        [Tooltip("Set replaces the value outright. Additive adds to the current value (Float/Int only).")]
        public ValueMode valueMode = ValueMode.Set;

        public float floatValue;
        public int intValue;
        public bool boolValue;
        public string stringValue;

        public override void Execute()
        {
            switch (targetVariable)
            {
                case FloatVariable f:
                    f.RuntimeValue = valueMode == ValueMode.Additive ? f.RuntimeValue + floatValue : floatValue;
                    break;

                case IntVariable i:
                    i.RuntimeValue = valueMode == ValueMode.Additive ? i.RuntimeValue + intValue : intValue;
                    break;

                case BoolVariable b:
                    b.RuntimeValue = boolValue;
                    break;

                case StringVariable s:
                    s.RuntimeValue = stringValue;
                    break;
            }
        }
    }
}
