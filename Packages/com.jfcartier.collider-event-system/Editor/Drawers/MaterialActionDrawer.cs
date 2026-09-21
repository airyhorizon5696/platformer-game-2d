using ColliderEventSystem;
using ColliderEventSystem.Editor;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Groups fields under "Target" and "Effect" section labels. Target Mode only shows when the owner
    /// has a ColliderEvent (Condition Watcher has no entering object, so it's locked to Specific Object).
    /// Target Renderer shows whenever Specific Object applies. New Material only shows when Mode is Apply
    /// - Restore Original needs nothing else, it puts back whatever Apply last remembered.
    /// </summary>
    public static class MaterialActionDrawer
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
                SerializedProperty targetRendererProp = so.FindProperty("targetRenderer");
                DrawField(cursor.NextField(targetRendererProp), targetRendererProp, draw);
            }

            EditorGUI.indentLevel--;
            cursor.Spacer();
            DrawSectionLabel(cursor.NextLine(), "Effect", draw);
            EditorGUI.indentLevel++;

            SerializedProperty modeProp = so.FindProperty("mode");
            DrawField(cursor.NextField(modeProp), modeProp, draw);

            if ((MaterialAction.Mode)modeProp.enumValueIndex == MaterialAction.Mode.Apply)
            {
                SerializedProperty newMaterialProp = so.FindProperty("newMaterial");
                DrawField(cursor.NextField(newMaterialProp), newMaterialProp, draw);
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
