using System;
using System.Collections.Generic;
using System.Linq;
using ColliderEventSystem;
using UnityEditor;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Finds every concrete Condition/Action type in the project via TypeCache (Editor-only, pre-indexed by
    /// Unity - no reflection scan needed). Backs every "Add Condition"/"Add Action" dropdown; a new
    /// Condition/Action class shows up automatically, nothing here needs to change.
    /// </summary>
    public static class ComponentTypeCache
    {
        public static IReadOnlyList<Type> ConditionTypes { get; } = GetConcreteTypes<ConditionBase>();
        public static IReadOnlyList<Type> ActionTypes { get; } = GetConcreteTypes<ActionBase>();

        private static IReadOnlyList<Type> GetConcreteTypes<T>()
        {
            return TypeCache.GetTypesDerivedFrom<T>()
                .Where(t => !t.IsAbstract)
                .OrderBy(DisplayName)
                .ToList();
        }

        // "InputCondition" -> "Input", "GameObjectAction" -> "GameObject" - the class name
        // still says Condition/Action (useful in code, where every type in the list needs to be told apart
        // from MonoBehaviours in general), but repeating that word in every single dropdown entry and
        // foldout header is just noise once they're already grouped under "Conditions"/"Actions".
        public static string DisplayName(Type type)
        {
            string name = type.Name;

            if (name.EndsWith("Condition") && name.Length > "Condition".Length)
            {
                name = name.Substring(0, name.Length - "Condition".Length);
            }
            else if (name.EndsWith("Action") && name.Length > "Action".Length)
            {
                name = name.Substring(0, name.Length - "Action".Length);
            }

            return NameFormatUtility.AddSpacesToPascalCase(name);
        }
    }
}
