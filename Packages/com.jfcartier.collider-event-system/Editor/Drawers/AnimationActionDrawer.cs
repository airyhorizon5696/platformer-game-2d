using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Groups fields under "Target" and "Effect" section labels. Target Mode only shows when the owner
    /// has a ColliderEvent (Condition Watcher has no entering object, so it's locked to Specific Object).
    /// Target shows whenever Specific Object applies, and Clip To Play/Trigger Name hide when Stop
    /// Animation is on - they're ignored in that case.
    /// </summary>
    public static class AnimationActionDrawer
    {
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
            var cursor = new RectLayoutCursor(rect);

            DrawSectionLabel(cursor.NextLine(), "Target", draw);
            EditorGUI.indentLevel++;

            SerializedProperty targetModeProp = so.FindProperty("targetMode");
            bool supportsEnteringObjects = TargetModeGui.SupportsEnteringObjects(so);

            if (supportsEnteringObjects)
            {
                DrawField(cursor.NextField(targetModeProp), targetModeProp, draw);
            }
            else
            {
                targetModeProp.enumValueIndex = (int)TargetMode.SpecificObject;
            }

            if (!supportsEnteringObjects || (TargetMode)targetModeProp.enumValueIndex == TargetMode.SpecificObject)
            {
                SerializedProperty targetProp = so.FindProperty("target");
                DrawField(cursor.NextField(targetProp), targetProp, draw);
            }

            EditorGUI.indentLevel--;
            cursor.Spacer();
            DrawSectionLabel(cursor.NextLine(), "Effect", draw);
            EditorGUI.indentLevel++;

            SerializedProperty stopProp = so.FindProperty("stopAnimation");
            DrawField(cursor.NextField(stopProp), stopProp, draw);

            if (!stopProp.boolValue)
            {
                SerializedProperty clipProp = so.FindProperty("clipToPlay");
                DrawField(cursor.NextField(clipProp), clipProp, draw);

                SerializedProperty triggerProp = so.FindProperty("triggerName");
                DrawField(cursor.NextField(triggerProp), triggerProp, draw);
            }

            EditorGUI.indentLevel--;

            return cursor.ConsumedHeight;
        }

        private static void DrawField(Rect rect, SerializedProperty property, bool draw)
        {
            if (draw) EditorGUI.PropertyField(rect, property);
        }

        private static void DrawSectionLabel(Rect rect, string text, bool draw)
        {
            if (draw) EditorGUI.LabelField(rect, text, EditorStyles.boldLabel);
        }
    }
}
