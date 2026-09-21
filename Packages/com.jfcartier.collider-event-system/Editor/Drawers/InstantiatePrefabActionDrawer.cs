using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Groups fields under "Target" and "Effect" section labels. Target Mode/Destination (where to spawn)
    /// draw first, then Prefab (what to spawn). Target Mode only shows when the owner has a ColliderEvent
    /// (Condition Watcher has no entering object, so it's locked to Specific Object). Destination shows
    /// whenever Specific Object applies.
    /// </summary>
    public static class InstantiatePrefabActionDrawer
    {
        private static readonly string[] SkipNames = { "prefab", "targetMode", "destination" };

        public static void Draw(Rect rect, SerializedObject so)
        {
            Layout(rect, so, true);
        }

        public static float GetHeight(SerializedObject so)
        {
            return Layout(default, so, false);
        }

        private static float Layout(Rect rect, SerializedObject so, bool draw)
        {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float y = rect.y;

            if (draw) EditorGUI.LabelField(new Rect(rect.x, y, rect.width, lineHeight), "Target", EditorStyles.boldLabel);
            y += lineHeight + 2f;

            EditorGUI.indentLevel++;

            SerializedProperty targetModeProp = so.FindProperty("targetMode");
            bool supportsEnteringObjects = TargetModeGui.SupportsEnteringObjects(so);

            if (supportsEnteringObjects)
            {
                if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), targetModeProp);
                y += lineHeight + 2f;
            }
            else
            {
                targetModeProp.enumValueIndex = (int)TargetMode.SpecificObject;
            }

            if (!supportsEnteringObjects || (TargetMode)targetModeProp.enumValueIndex == TargetMode.SpecificObject)
            {
                SerializedProperty destinationProp = so.FindProperty("destination");
                if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), destinationProp);
                y += lineHeight + 2f;
            }

            EditorGUI.indentLevel--;
            y += 6f;

            if (draw) EditorGUI.LabelField(new Rect(rect.x, y, rect.width, lineHeight), "Effect", EditorStyles.boldLabel);
            y += lineHeight + 2f;

            EditorGUI.indentLevel++;

            SerializedProperty prefabProp = so.FindProperty("prefab");
            if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), prefabProp);
            y += lineHeight + 2f;

            Rect remainingRect = new Rect(rect.x, y, rect.width, rect.height);
            y += draw
                ? ConditionActionListDrawer.DrawRemainingFields(remainingRect, so, SkipNames)
                : ConditionActionListDrawer.GetRemainingFieldsHeight(so, SkipNames);

            EditorGUI.indentLevel--;

            return y - rect.y;
        }
    }
}
