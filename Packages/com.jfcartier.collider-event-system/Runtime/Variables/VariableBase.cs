namespace ColliderEventSystem
{
    /// <summary>
    /// Typed half of the Variable split. Initial Value is serialized and edited in the Inspector.
    /// Runtime Value is marked [NonSerialized] on purpose: a plain serialized ScriptableObject field
    /// would NOT auto-revert after Play Mode the way a scene object's fields do (a common surprise),
    /// so gameplay changes only ever touch Runtime Value, and Initial Value on disk never changes
    /// because of something that happened in-game.
    /// </summary>
    public abstract class VariableBase<T> : Variable
    {
        [UnityEngine.SerializeField] private T m_InitialValue;

        public T InitialValue => m_InitialValue;

        [System.NonSerialized] private T m_RuntimeValue;

        /// <summary>
        /// The live value read/written during gameplay by VariableCondition and VariableAction.
        /// </summary>
        public T RuntimeValue
        {
            get => m_RuntimeValue;
            set
            {
                m_RuntimeValue = value;

                if (Persistent)
                {
                    SavePersisted();
                }
            }
        }

        public override void ResetRuntimeValue()
        {
            m_RuntimeValue = m_InitialValue;
        }
    }
}
