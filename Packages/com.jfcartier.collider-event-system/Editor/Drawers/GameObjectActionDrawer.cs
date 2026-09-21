using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Groups fields under "Target" and "Effect" section labels. Target Mode only shows when the owner
    /// has a ColliderEvent (Condition Watcher has no entering object, so it's locked to Specific Object).
    /// Target shows whenever Specific Object applies. Everything under Effect (Operation) draws generically.
    /// </summary>
    public static class GameObjectActionDrawer
    {
        private static readonly string[] SkipNames = { "targetMode", "target" };

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
                SerializedProperty targetProp = so.FindProperty("target");
                if (draw) EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, lineHeight), targetProp);
                y += lineHeight + 2f;
            }

            EditorGUI.indentLevel--;
            y += 6f;

            if (draw) EditorGUI.LabelField(new Rect(rect.x, y, rect.width, lineHeight), "Effect", EditorStyles.boldLabel);
            y += lineHeight + 2f;

            EditorGUI.indentLevel++;

            Rect remainingRect = new Rect(rect.x, y, rect.width, rect.height);
            y += draw
                ? ConditionActionListDrawer.DrawRemainingFields(remainingRect, so, SkipNames)
                : ConditionActionListDrawer.GetRemainingFieldsHeight(so, SkipNames);

            EditorGUI.indentLevel--;

            return y - rect.y;
        }
    }
}
