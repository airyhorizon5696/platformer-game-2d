using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor
{
    /// <summary>
    /// Shared "Entering Objects" eligibility check for every Action/Condition drawer that has a Target
    /// Mode field. Condition Watcher has no physical zone, so it can never provide an entering object -
    /// Target Mode should never even offer that choice there, not just warn once it's picked.
    /// </summary>
    public static class TargetModeGui
    {
        public static bool SupportsEnteringObjects(SerializedObject elementSo)
        {
            var component = elementSo.targetObject as Component;
            return component != null && component.GetComponent<ColliderEvent>() != null;
        }
    }
}
