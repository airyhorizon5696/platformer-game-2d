using ColliderEventSystem;
using UnityEditor;
using UnityEngine;

namespace ColliderEventSystem.Editor.Drawers
{
    /// <summary>
    /// Shows Relative To only when Space is Relative To Transform.
    /// </summary>
    public static class RotationConditionDrawer
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

            SerializedProperty targetProp = so.FindProperty("target");
            DrawField(cursor.NextField(targetProp), targetProp, draw);

            SerializedProperty spaceProp = so.FindProperty("space");
            DrawField(cursor.NextField(spaceProp), spaceProp, draw);

            if ((RotationCondition.RotationSpace)spaceProp.enumValueIndex == RotationCondition.RotationSpace.RelativeToTransform)
            {
                SerializedProperty relativeToProp = so.FindProperty("relativeTo");
                DrawField(cursor.NextField(relativeToProp), relativeToProp, draw);
            }

            SerializedProperty axisProp = so.FindProperty("axis");
            DrawField(cursor.NextField(axisProp), axisProp, draw);

            SerializedProperty operatorProp = so.FindProperty("operatorValue");
            DrawField(cursor.NextField(operatorProp), operatorProp, draw);

            SerializedProperty valueProp = so.FindProperty("value");
            DrawField(cursor.NextField(valueProp), valueProp, draw);

            return cursor.ConsumedHeight;
        }

        private static void DrawField(Rect rect, SerializedProperty property, bool draw)
        {
            if (draw) EditorGUI.PropertyField(rect, property);
        }
    }
}
