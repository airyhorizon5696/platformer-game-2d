using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ColliderEventSystem
{
    /// <summary>
    /// Base class for every Variable asset (FloatVariable, IntVariable, BoolVariable, StringVariable).
    /// Drag one of these into a VariableCondition or VariableAction instead of typing a string key.
    /// </summary>
    public abstract class Variable : ScriptableObject
    {
        [SerializeField, HideInInspector] private string m_Id;

        /// <summary>
        /// A stable, auto-generated identifier for this variable. Used as the PlayerPrefs key when
        /// Persistent is enabled. Never typed by hand, so it can't have a typo.
        /// </summary>
        public string Id => m_Id;

        [Tooltip("If true, this variable's value is saved to disk and reloaded automatically, surviving between play sessions and app restarts. If false (default), it always starts fresh from Initial Value.")]
        public bool Persistent;

        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(m_Id))
            {
                m_Id = Guid.NewGuid().ToString("N");
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }

            ResetRuntimeValue();

            if (Persistent)
            {
                LoadPersisted();
            }
        }

        /// <summary>
        /// Resets the runtime value back to the initial value shown in the Inspector. Never touches
        /// PlayerPrefs - call LoadPersisted() separately if a saved value should override it.
        /// </summary>
        public abstract void ResetRuntimeValue();

        /// <summary>
        /// Loads a previously saved value from PlayerPrefs into the runtime value, if one exists.
        /// </summary>
        protected abstract void LoadPersisted();

        /// <summary>
        /// Saves the current runtime value to PlayerPrefs.
        /// </summary>
        protected abstract void SavePersisted();

        /// <summary>
        /// Deletes any saved PlayerPrefs value for this variable and resets the runtime value.
        /// </summary>
        public void ClearPersisted()
        {
            PlayerPrefs.DeleteKey(Id);
            PlayerPrefs.Save();
            ResetRuntimeValue();
        }
    }
}
