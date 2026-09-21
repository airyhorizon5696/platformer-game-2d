using ColliderEventSystem;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Shows the Key field or the Action Reference field depending on Source.
    /// </summary>
    public static class InputConditionDrawer
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

            SerializedProperty sourceProp = so.FindProperty("source");
            DrawField(cursor.NextField(sourceProp), sourceProp, draw);

            var source = (InputCondition.InputSource)sourceProp.enumValueIndex;

            if (source == InputCondition.InputSource.Key)
            {
                SerializedProperty keyProp = so.FindProperty("key");
                DrawField(cursor.NextField(keyProp), keyProp, draw);
            }
            else
            {
                SerializedProperty actionProp = so.FindProperty("actionReference");
                DrawField(cursor.NextField(actionProp), actionProp, draw);
            }

            SerializedProperty triggerOnProp = so.FindProperty("triggerOn");
            Rect triggerOnRect = cursor.NextField(triggerOnProp);
            if (draw) EditorGUI.PropertyField(triggerOnRect, triggerOnProp, new GUIContent("Trigger On"));

            return cursor.ConsumedHeight;
        }

        private static void DrawField(Rect rect, SerializedProperty property, bool draw)
        {
            if (draw) EditorGUI.PropertyField(rect, property);
        }
    }
}
